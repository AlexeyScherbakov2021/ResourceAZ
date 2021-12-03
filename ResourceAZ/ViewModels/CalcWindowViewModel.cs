using OxyPlot;
using ResourceAZ.Models;
using ResourceAZ.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResourceAZ.ViewModels;
using OxyPlot.Series;
using OxyPlot.Axes;

namespace ResourceAZ.ViewModels
{
    internal partial class CalcWindowViewModel : ViewModel
    {
        // переданные данные из исходных данных
        private KindGroup SelectGroup;
        private KindCalc SelectCalc;
        public double MinSummPot { get; set; }
        private ObservableCollection<Measure> InputMeasure;
        public double deltaA { get; set; }
        public double deltaR { get; set; }

        public PlotModel ModelCurrent { get; }
        public PlotModel ModelNapr { get; }
        ObservableCollection<DataPoint> dpCurrent { get; set; }
        ObservableCollection<DataPoint> dpNapr { get; set; }


        // рассчитанные данные
        private ObservableCollection<Measure> _listMeasure;
        public ObservableCollection<Measure> listMeasure
        {
            get => _listMeasure;
            set
            {
                Set(ref _listMeasure, value);
                //ModelToChart(listMeasure);
            }
        }

        DateTime StartDateTime;
        DateTime EndDateTime;


        public CalcWindowViewModel()
        {
            listMeasure = new ObservableCollection<Measure>();

            ModelCurrent = new PlotModel();
            ModelNapr = new PlotModel();


        }

        //public CalcWindowViewModel(KindGroup kg, KindCalc kc,double MinPot, ObservableCollection<Measure> measure) : this()
        //SelectGroup = kg;
        //    SelectCalc = kc;
        //    InputMeasure = measure;
        //    MinSummPot = MinPot;

        public CalcWindowViewModel(MainWindowViewModel model) : this()
        {
            SelectGroup = model.SelectGroup;
            SelectCalc = model.LineApprox ? KindCalc.ApprLine : KindCalc.EndPoints;
            InputMeasure = model.listMeasure;
            MinSummPot = model.MinPotCalc;

            if(SelectCalc == KindCalc.ApprLine)
            {
                // расчет по линии аппроксимации

                double StartValueA = InputMeasure[0].Koeff;
                double EndValueA = InputMeasure[InputMeasure.Count - 1].Koeff;
                double StartValueR = InputMeasure[0].Resist;
                double EndValueR = InputMeasure[InputMeasure.Count - 1].Resist;

                DateTime EndDate = InputMeasure[InputMeasure.Count - 1].date;
                DateTime StartDate = InputMeasure[0].date;



                TimeSpan dateSub = EndDate.Subtract(StartDate);
                double Years = dateSub.Days / 365;

                double deltaA = (StartValueA - EndValueA) / Years;
                double deltaR = (StartValueR - EndValueR) / Years;

                do
                {
                    Measure meas = new Measure();
                    EndValueA -= deltaA;
                    EndValueR -= deltaR;
                    if (EndValueA >= 0 || EndValueR <= 0)
                        break;
                    EndDate = EndDate.AddYears(1);
                    meas.date = EndDate;
                    meas.Koeff = EndValueA;
                    meas.Resist = EndValueR;
                    meas.Current = MinSummPot / meas.Koeff;
                    meas.Napr = meas.Current * meas.Resist;
                    listMeasure.Add(meas);

                } while (listMeasure[listMeasure.Count - 1].Current <= model.MaxCurrentSKZ || 
                                listMeasure[listMeasure.Count - 1].Napr <= model.MaxNaprSKZ);

                StartDateTime = InputMeasure[InputMeasure.Count - 1].date.AddYears(1);
                EndDateTime = EndDate;

            }
            else
            {
                // расчет по крайним точкам

                

            }

            dpCurrent = InitChart(ModelCurrent, "Выходной ток", model.MaxCurrentSKZ);
            dpNapr = InitChart(ModelNapr, "Напряжение", model.MaxNaprSKZ);
            // занесение точек в графики и отображение их
            ModelToChart(listMeasure);

        }

        //--------------------------------------------------------------------------------------------
        // перенос данных а график
        //--------------------------------------------------------------------------------------------
        private void ModelToChart(ObservableCollection<Measure> meas)
        {
            dpCurrent.Clear();
            dpNapr.Clear();

            // заполнение точками срисков для графиков
            foreach (Measure m in meas)
            {
                dpCurrent.Add(new DataPoint(m.date.ToOADate(), m.Current));
                dpNapr.Add(new DataPoint(m.date.ToOADate(), m.Napr));

            }

            // обновление точек графиков на экране
            ModelCurrent.InvalidatePlot(true);
            ModelNapr.InvalidatePlot(true);
        }

        public ObservableCollection<DataPoint> InitChart(PlotModel model, string title, double MaxLine)
        {

            ObservableCollection<DataPoint> dp = new ObservableCollection<DataPoint>();
            LineSeries ls = new LineSeries();
            ls.ItemsSource = dp;
            model.Series.Add(ls);
            ls.Color = OxyColor.FromRgb(0, 0, 255);
            ls.MarkerType = MarkerType.Circle;
            ls.StrokeThickness = 3;
            var XAxis = new DateTimeAxis();
            XAxis.AxislineStyle = LineStyle.Dot;
            XAxis.StringFormat = "yyyy";
            XAxis.MajorGridlineStyle = LineStyle.Automatic;
            XAxis.MinorGridlineStyle = LineStyle.Dot;
            XAxis.FontSize = 11;
            model.Axes.Add(XAxis);
            var YAxis = new LinearAxis();
            YAxis.MajorGridlineStyle = LineStyle.Automatic;
            YAxis.MinorGridlineStyle = LineStyle.Dot;
            YAxis.FontSize = 11;
            model.Title = title;
            model.Axes.Add(YAxis);
            model.TitleFontSize = 13;
            model.PlotMargins = new OxyThickness(20, 0, 5, 20);

            ls = new LineSeries();
            ObservableCollection<DataPoint> dpMax = new ObservableCollection<DataPoint>();
            DataPoint d = new DataPoint(StartDateTime.ToOADate(), MaxLine);
            dpMax.Add(d);
            d = new DataPoint(EndDateTime.ToOADate(), MaxLine);
            dpMax.Add(d);
            ls.ItemsSource = dpMax;
            ls.Color = OxyColor.FromRgb(255, 0, 0);
            ls.StrokeThickness = 3;
            model.Series.Add(ls);

            return dp;
        }

    }
}
