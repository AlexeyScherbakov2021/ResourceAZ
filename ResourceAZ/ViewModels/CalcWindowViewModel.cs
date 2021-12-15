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
using ResourceAZ.Calculation;

namespace ResourceAZ.ViewModels
{
    internal partial class CalcWindowViewModel : ViewModel
    {
        // переданные данные из исходных данных
        private KindGroup SelectGroup;
        private KindCalc SelectCalc;
        public double MinSummPot { get; set; }
        private ObservableCollection<Measure> InputMeasure;
        //public double deltaA { get; set; }
        //public double deltaR { get; set; }
        public string textResult { get; set; }

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

        //DateTime StartDateTime;
        //DateTime EndDateTime;


        public CalcWindowViewModel()
        {
            //listMeasure = new ObservableCollection<Measure>();

            ModelCurrent = new PlotModel();
            ModelNapr = new PlotModel();
        }

        public CalcWindowViewModel(MainWindowViewModel model) : this()
        {
            SelectGroup = model.SelectGroup;
            SelectCalc = model.CalcPotencial ? KindCalc.Potencial : KindCalc.Resist;
            InputMeasure = model.listMeasure;
            MinSummPot = model.MinPotCalc;
            ICalculateBase calc = null;

            if (SelectCalc == KindCalc.Potencial)
                // расчет по линии потенциалам
                calc = new CalculatePotencial();

            else if(SelectCalc == KindCalc.Resist)
                // расчет по сопротивлениям
                calc = new CalculateResist();

            if (calc == null)
                throw new Exception("Не определен тип расчета.");

            listMeasure = calc.Calc(model.listMeasure, model.MinPotCalc, model.MaxCurrentSKZ, model.MaxNaprSKZ);

            if (listMeasure.Count > 0)
            {
                // добавление в график максимальных линий
                dpCurrent = InitChart(ModelCurrent, $"Выходной ток. Макимальный {model.MaxCurrentSKZ} А.",
                    model.MaxCurrentSKZ, listMeasure[listMeasure.Count - 1].Current);
                dpNapr = InitChart(ModelNapr, $"Напряжение. Максимальное {model.MaxNaprSKZ} В.",
                    model.MaxNaprSKZ, listMeasure[listMeasure.Count - 1].Napr);
                // занесение точек в графики и их отображение 
                ModelToChart(listMeasure);

                Measure meas = listMeasure.Where(m => m.Current >= model.MaxCurrentSKZ).FirstOrDefault();
                int LimitYearCurr = meas?.date.Year - 1 ?? listMeasure[listMeasure.Count - 1].date.Year - 1;
                meas = listMeasure.Where(m => m.Napr >= model.MaxNaprSKZ).FirstOrDefault();
                int LimitYearNapr = meas?.date.Year - 1 ?? listMeasure[listMeasure.Count - 1].date.Year - 1;

                textResult = $"Предельный режим СКЗ для анодного заземлителя будет достигнут в { Math.Min(LimitYearCurr, LimitYearNapr)} году.\n" +
                    $"По току в {LimitYearCurr} году\n" +
                    $"По напряжению в {LimitYearNapr} году";
                //$"Максимальноый ток СКЗ {model.MaxCurrentSKZ} А.\n" +
                //$"Максимальное напряжение СКЗ {model.MaxNaprSKZ} В.";
            }
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


        //--------------------------------------------------------------------------------------------
        // инициализация графика
        //--------------------------------------------------------------------------------------------
        public ObservableCollection<DataPoint> InitChart(PlotModel model, string title, double MaxLine, double Maximum)
        {
            DateTime StartDateTime = listMeasure[0].date;
            DateTime EndDateTime = listMeasure[listMeasure.Count - 1].date;

            ObservableCollection<DataPoint> dp = new ObservableCollection<DataPoint>();
            LineSeries ls = new LineSeries();
            
            ls.ItemsSource = dp;
            ls.Color = OxyColors.Blue;
            //ls.MarkerType = MarkerType.Circle;
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
            YAxis.Maximum = Maximum;
            model.Axes.Add(YAxis);
            model.TitleFontSize = 13;
            model.PlotMargins = new OxyThickness(20, 0, 5, 20);

            AreaSeries ar = new AreaSeries();
            ObservableCollection<DataPoint> dpMax = new ObservableCollection<DataPoint>();
            //DataPoint d = new DataPoint(StartDateTime.ToOADate(), MaxLine);
            //dpMax.Add(d);
            //d = new DataPoint(EndDateTime.ToOADate(), MaxLine);
            //dpMax.Add(d);
            //ar.ItemsSource = dpMax;
            ar.Color = OxyColor.FromArgb(255, 255, 150, 150);
            ar.StrokeThickness = 1;
            ar.Points.Add(new DataPoint(StartDateTime.ToOADate(), MaxLine));
            ar.Points.Add(new DataPoint(EndDateTime.ToOADate(), MaxLine));
            ar.Points2.Add(new DataPoint(StartDateTime.ToOADate(), MaxLine + 500));
            ar.Points2.Add(new DataPoint(EndDateTime.ToOADate(), MaxLine + 500));
            //var YAxis2 = new LinearAxis();
            //YAxis2.Title = "Линия";
            //YAxis2.Key = "fffff";
            //YAxis2.Unit = "0000000000000";
            //YAxis2.Position = AxisPosition.Right;
            //model.Axes.Add(YAxis2);

            model.Series.Add(ar);
            model.Series.Add(ls);

            //HighLowSeries hs = new HighLowSeries();
            //hs.ItemsSource = dpMax;
            //model.Series.Add(hs);

            return dp;
        }

    }
}
