//using OxyPlot;
using ResourceAZ.Models;
using ResourceAZ.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResourceAZ.ViewModels;
//using OxyPlot.Series;
//using OxyPlot.Axes;
using ResourceAZ.Calculation;
using ResourceAZ.ScottChart;
using ScottPlot;
using ScottPlot.Plottable;
using ResourceAZ.Views;
using System.Windows.Input;
using ResourceAZ.Infrastructure.Commands;
using System.Drawing;

namespace ResourceAZ.ViewModels
{
    internal partial class CalcWindowViewModel : ViewModel
    {
        public CalcWindow view;
        
        // переданные данные из исходных данных
        private KindGroup SelectGroup;
        private KindCalc SelectCalc;
        public double MinSummPot { get; set; }
        private ObservableCollection<Measure> InputMeasure;
        private List<string> _lbResult;
        public List<string> lbResult { get => _lbResult; set { Set(ref _lbResult, value); } }

        double MaxCurrent;
        double MaxNapr;

        //public PlotModel ModelCurrent { get; }
        //public PlotModel ModelNapr { get; }
        //ObservableCollection<DataPoint> dpCurrent { get; set; }
        //ObservableCollection<DataPoint> dpNapr { get; set; }
        CalculateBase calc = null;

        //ScatterPlot plotCurrent;
        //ScatterPlot plotNapr;

        double[] currents;
        double[] naprs;
        double[] dates;

        // рассчитанные данные
        private ObservableCollection<Measure> _listMeasure;
        public ObservableCollection<Measure> listMeasure
        {
            get => _listMeasure;
            set
            {
                Set(ref _listMeasure, value);
            }
        }


        public ICommand CommandLoaded { get; }
        private bool CanCommandLoadedCommand(object p) => true;

        private void OnCommandLoadedCommand(object p)
        {
            // выполнение расчета
            listMeasure = calc.Calc();

            if (listMeasure.Count > 0)
            {
                // занесение точек в графики и их отображение 
                ModelToChart(listMeasure);

                lbResult = calc.ResultText();
            }
        }



        public CalcWindowViewModel()
        {
            //ModelCurrent = new PlotModel();
            //ModelNapr = new PlotModel();
            CommandLoaded = new LambdaCommand(OnCommandLoadedCommand, CanCommandLoadedCommand);

        }

        public CalcWindowViewModel(MainWindowViewModel model) : this()
        {
            SelectGroup = model.SelectGroup;
            SelectCalc = model.CalcPotencial ? KindCalc.Potencial : KindCalc.Resist;
            // список данных для расчета
            InputMeasure = model.listMeasure;
            // минимальный суммарный потенциал для расчета по потенциалу
            MinSummPot = model.MinPotCalc;
            // максимальные параметры СКЗ
            MaxCurrent = model.MaxCurrentSKZ;
            MaxNapr = model.MaxNaprSKZ;

            if (SelectCalc == KindCalc.Potencial)
                // расчет по линии потенциалам
                calc = new CalculatePotencial(model);

            else if (SelectCalc == KindCalc.Resist)
            {
                // расчет по сопротивлениям
                calc = new CalculateResist(model);
                MaxCurrent = -1;
            }

            if (calc == null)
                throw new ArgumentOutOfRangeException("Не определен тип расчета.");

            //// выполнение расчета
            //listMeasure = calc.Calc();

            //if (listMeasure.Count > 0)
            //{
            //    // занесение точек в графики и их отображение 
            //    ModelToChart(listMeasure);

            //    lbResult = calc.ResultText();
            //}
        }

        //--------------------------------------------------------------------------------------------
        // перенос данных а график
        //--------------------------------------------------------------------------------------------
        private void ModelToChart(ObservableCollection<Measure> meas)
        {
            //dpCurrent.Clear();
            //dpNapr.Clear();

            // заполнение точками списков для графиков
            //foreach (Measure m in meas)
            //{
            //    dpCurrent.Add(new DataPoint(m.date.ToOADate(), m.Current));
            //    dpNapr.Add(new DataPoint(m.date.ToOADate(), m.Napr));

            //}

            // обновление точек графиков на экране
            //ModelCurrent.InvalidatePlot(true);
            //ModelNapr.InvalidatePlot(true);

            currents = new double[meas.Count];
            naprs = new double[meas.Count];
            dates = new double[meas.Count];

            int i = 0;
            foreach (Measure m in meas)
            {
                dates[i] = m.date.ToOADate();
                currents[i] = m.Current;
                naprs[i] = m.Napr;
                i++;
            }

            ScatterPlot plotCurr = view.PlotCurrent.Plot.AddScatter(dates, currents);
            view.PlotCurrent.Plot.Title($"Выходной ток, максимальный: {MaxCurrent}", false);
            view.PlotCurrent.Plot.XAxis.DateTimeFormat(true);
            view.PlotCurrent.Plot.XAxis.Grid(true);
            view.PlotCurrent.Plot.Layout(0, 0, 0, 0, -3);
            view.PlotCurrent.Plot.Grid(true, Color.LightGray);
            plotCurr.LineWidth = 3;
            view.PlotCurrent.Plot.AddHorizontalLine(MaxCurrent, Color.Red, 3, LineStyle.Dash, "Макс. ток");
            view.PlotCurrent.Refresh();

            ScatterPlot plotNapr = view.PlotNapr.Plot.AddScatter(dates, naprs);
            view.PlotNapr.Plot.Title($"Напряжение, максимальное: {MaxNapr}", false);
            plotNapr.LineWidth = 3;
            view.PlotNapr.Plot.XAxis.DateTimeFormat(true);
            view.PlotNapr.Plot.XAxis.Grid(true);
            view.PlotNapr.Plot.Layout(0, 0, 0, 0, -3);
            view.PlotNapr.Plot.Grid(true, Color.LightGray);
            view.PlotNapr.Plot.AddHorizontalLine(MaxNapr, Color.Red, 3, LineStyle.Dash, "Макс. напряжение");
            view.PlotNapr.Refresh();
        }


        //--------------------------------------------------------------------------------------------
        // инициализация графика
        //--------------------------------------------------------------------------------------------
        //public ObservableCollection<DataPoint> InitChart(PlotModel model, string title, double MaxLine, double Maximum)
        //{
        //    DateTime StartDateTime = listMeasure[0].date;
        //    DateTime EndDateTime = listMeasure[listMeasure.Count - 1].date;

        //    ObservableCollection<DataPoint> dp = new ObservableCollection<DataPoint>();
        //    LineSeries ls = new LineSeries();
            
        //    ls.ItemsSource = dp;
        //    ls.Color = OxyColors.Blue;
        //    //ls.MarkerType = MarkerType.Circle;
        //    ls.StrokeThickness = 3;
        //    var XAxis = new DateTimeAxis();
        //    XAxis.AxislineStyle = LineStyle.Dot;
        //    XAxis.StringFormat = "yyyy";
        //    XAxis.MajorGridlineStyle = LineStyle.Automatic;
        //    XAxis.MinorGridlineStyle = LineStyle.Dot;
        //    XAxis.FontSize = 11;
        //    model.Axes.Add(XAxis);
        //    var YAxis = new LinearAxis();
        //    YAxis.MajorGridlineStyle = LineStyle.Automatic;
        //    YAxis.MinorGridlineStyle = LineStyle.Dot;
        //    YAxis.FontSize = 11;
        //    model.Title = title;
        //    YAxis.Maximum = Maximum;
        //    model.Axes.Add(YAxis);
        //    model.TitleFontSize = 13;
        //    model.PlotMargins = new OxyThickness(20, 0, 5, 20);

        //    if (MaxLine > 0)
        //    {
        //        AreaSeries ar = new AreaSeries();
        //        ObservableCollection<DataPoint> dpMax = new ObservableCollection<DataPoint>();
        //        ar.Color = OxyColor.FromArgb(255, 255, 150, 150);
        //        ar.StrokeThickness = 1;
        //        ar.Points.Add(new DataPoint(StartDateTime.ToOADate(), MaxLine));
        //        ar.Points.Add(new DataPoint(EndDateTime.ToOADate(), MaxLine));
        //        ar.Points2.Add(new DataPoint(StartDateTime.ToOADate(), MaxLine + 5000));
        //        ar.Points2.Add(new DataPoint(EndDateTime.ToOADate(), MaxLine + 5000));
        //        model.Series.Add(ar);
        //    }

        //    model.Series.Add(ls);

        //    return dp;
        //}


    }
}
