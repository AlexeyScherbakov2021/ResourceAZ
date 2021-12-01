using ResourceAZ.Infrastructure.Commands;
using ResourceAZ.Models;
using ResourceAZ.Repository;
using ResourceAZ.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using OxyPlot;
using OxyPlot.Series;
//using LiveCharts;
//using LiveCharts.Wpf;
using System.Windows.Media;
using OxyPlot.Axes;
using System.Collections.ObjectModel;
using ResourceAZ.Views;

namespace ResourceAZ.ViewModels
{
    //enum TypeMeasure { CURRENT,  NAPR, POTENCIAL };

     internal class MainWindowViewModel : ViewModel
    {
        // оригинал полученного списка измерений
        private ObservableCollection<Measure> listMeasureOrig;
        // сгрупированный список измерений
        private ObservableCollection<Measure> _listMeasure;
        public ObservableCollection<Measure> listMeasure
        {
            get => _listMeasure;
            set
            {
                Set(ref _listMeasure, value);
                ModelToChart(listMeasure);
            }
        }

        // измерения, преобраованные в списки точек для графика
        #region
        public ObservableCollection<DataPoint> dpCurrent { get; set; }
        ObservableCollection<DataPoint> dpNapr { get; set; }
        ObservableCollection<DataPoint> dpPot { get; set; }
        // модели для графиков
        public PlotModel ModelCurrent { get; }
        public PlotModel ModelNapr { get; }
        public PlotModel ModelPot { get; }
        #endregion

        // база данных измерений
        private IMeasureData repository = new MeasureDataGen();


        public double MinPotCalc { get; set; } = -1.5;

        private KindGroup SelectGroup;

        // переменные связанные с экраннй формой
        #region
        private bool _GroupNone;
        public bool GroupNone
        {
            get => _GroupNone;
            set
            {
                _GroupNone = value;
                if (value)
                {
                    FormatListMeasure(KindGroup.NONE);
                    SelectGroup = KindGroup.NONE;
                }
            }
        } 
        private bool _GroupYear;
        public bool GroupYear
        {
            get => _GroupYear;
            set
            {
                _GroupYear = value;
                if (value)
                { 
                    FormatListMeasure(KindGroup.YEAR);
                    SelectGroup = KindGroup.YEAR;
                }
        }
    }

        private bool _GroupMonth;
        public bool GroupMonth
        {
            get => _GroupMonth;
            set
            {
                _GroupMonth = value;
                if(value)
                { 
                    FormatListMeasure(KindGroup.MONTH);
                    SelectGroup = KindGroup.MONTH;
                }
            }
        }

        private bool _GroupDay;
        public bool GroupDay
        {
            get => _GroupDay;
            set
            {
                _GroupDay = value;
                if (value)
                { 
                    FormatListMeasure(KindGroup.DAY);
                    SelectGroup = KindGroup.DAY;
                }
            }
        }

        public bool RemoveBadValue { get; set; }

        public bool LineApprox { get; set; }
        public bool EndPoints { get; set; } = true;

        #endregion

        #region
        #endregion


        #region Команды
        public ICommand CloseApplicationCommand { get; }
        private bool CanCloseApplicationCommand(object p) => true;

        private void OnCloseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }


        // команда на расчеты
        public ICommand CalculateCommand { get; }
        private bool CanCalculateCommand(object p)
        {
            return listMeasure.Count > 2;
        }
        private void OnCalculateCommand(object p)
        {
            KindCalc kc = LineApprox ? KindCalc.ApprLine : KindCalc.EndPoints;
            double LastR = listMeasure[listMeasure.Count - 1].Resist;
            double LastA = listMeasure[listMeasure.Count - 1].Koeff;

            CalcWindow calcWin = new CalcWindow(SelectGroup, kc, LastA, LastR, MinPotCalc);
            calcWin.ShowDialog();

        }

        #endregion

        public MainWindowViewModel()
        {
            // инициализация команд
            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommand);
            CalculateCommand = new LambdaCommand(OnCalculateCommand, CanCalculateCommand);

            // подгтовка графиков для всех измерений
            ModelCurrent = new PlotModel();
            ModelNapr = new PlotModel();
            ModelPot = new PlotModel();

            InitChart();

            // получение списка значений
            listMeasureOrig = repository.GetAllData();
            listMeasure = new ObservableCollection<Measure>(listMeasureOrig);

            // отмечаем на экране первый RadioButton
            GroupNone = true;
        }


        //--------------------------------------------------------------------------------------------
        // первоначальная инициализация графиков
        //--------------------------------------------------------------------------------------------
        private void InitChart()
        {

            // инициализация графика тока
            dpCurrent = new ObservableCollection<DataPoint>();
            LineSeries ls = new LineSeries();
            ls.ItemsSource = dpCurrent;
            ModelCurrent.Series.Add(ls);
            ls.Color = OxyColor.FromRgb(0, 0, 255);
            ls.MarkerType = MarkerType.Circle;
            ls.StrokeThickness = 3;
            var XAxis = new DateTimeAxis();
            XAxis.AxislineStyle = LineStyle.Dot;
            XAxis.StringFormat = "dd.MM.yyyy";
            XAxis.MajorGridlineStyle = LineStyle.Automatic;
            XAxis.MinorGridlineStyle = LineStyle.Dot;
            ModelCurrent.Axes.Add(XAxis);
            var YAxis = new LinearAxis();
            YAxis.MajorGridlineStyle = LineStyle.Automatic;
            YAxis.MinorGridlineStyle = LineStyle.Dot;
            ModelCurrent.Title = "Выходной ток";
            ModelCurrent.Axes.Add(YAxis);

            // инициализация графика напряжения
            dpNapr = new ObservableCollection<DataPoint>();
            ls = new LineSeries();
            ls.ItemsSource = dpNapr;
            ModelNapr.Series.Add(ls);
            ls.Color = OxyColor.FromRgb(0, 0, 255);
            ls.MarkerType = MarkerType.Circle;
            ls.StrokeThickness = 3;
            XAxis = new DateTimeAxis();
            XAxis.AxislineStyle = LineStyle.Dot;
            XAxis.StringFormat = "dd.MM.yyyy";
            XAxis.MajorGridlineStyle = LineStyle.Automatic;
            XAxis.MinorGridlineStyle = LineStyle.Dot;
            ModelNapr.Axes.Add(XAxis);
            YAxis = new LinearAxis();
            YAxis.MajorGridlineStyle = LineStyle.Automatic;
            YAxis.MinorGridlineStyle = LineStyle.Dot;
            ModelNapr.Title = "Напряжение";
            ModelNapr.Axes.Add(YAxis);

            // инициализация графика потенциала
            dpPot = new ObservableCollection<DataPoint>();
            ls = new LineSeries();
            ls.ItemsSource = dpPot;
            ModelPot.Series.Add(ls);
            ls.Color = OxyColor.FromRgb(0, 0, 255);
            ls.MarkerType = MarkerType.Circle;
            ls.StrokeThickness = 3;
            XAxis = new DateTimeAxis();
            XAxis.AxislineStyle = LineStyle.Dot;
            XAxis.StringFormat = "dd.MM.yyyy";
            XAxis.MajorGridlineStyle = LineStyle.Automatic;
            XAxis.MinorGridlineStyle = LineStyle.Dot;
            ModelPot.Axes.Add(XAxis);
            YAxis = new LinearAxis();
            YAxis.MajorGridlineStyle = LineStyle.Automatic;
            YAxis.MinorGridlineStyle = LineStyle.Dot;
            ModelPot.Title = "Потенциал";
            ModelPot.Axes.Add(YAxis);

        }

        //--------------------------------------------------------------------------------------------
        // создание графиков и перенос данных а график
        //--------------------------------------------------------------------------------------------
        private void ModelToChart(ObservableCollection<Measure> meas)
        {
            dpCurrent.Clear();
            dpNapr.Clear();
            dpPot.Clear();

            // заполнение точками срисков для графиков
            foreach (Measure m in meas)
            {
                dpCurrent.Add(new DataPoint(m.date.ToOADate(), m.Current));
                dpNapr.Add(new DataPoint(m.date.ToOADate(), m.Napr));
                dpPot.Add(new DataPoint(m.date.ToOADate(), m.SummPot));

            }

            // обновление точек графиков на экране
            ModelCurrent.InvalidatePlot(true);
            ModelNapr.InvalidatePlot(true);
            ModelPot.InvalidatePlot(true);
        }


        //--------------------------------------------------------------------------------------------
        // формирование списка по критериям
        //--------------------------------------------------------------------------------------------
        private void FormatListMeasure(KindGroup kind)
        {
            IEnumerable<Measure> group = null;

            switch(kind)
            {
                case KindGroup.NONE:
                    listMeasure = new ObservableCollection<Measure>(listMeasureOrig);
                    return;

                case KindGroup.DAY:
                    group = from Measure in listMeasureOrig
                                   group Measure
                                   by new { Measure.date.Year, Measure.date.Month, Measure.date.Day }
                                into g
                                   let avgCurr = g.Average(a => a.Current)
                                   let avgNapr = g.Average(a => a.Napr)
                                   let avgPot = g.Average(a => a.SummPot)
                                   select new Measure
                                   {
                                       Current = avgCurr,
                                       date = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                                       Napr = avgNapr,
                                       SummPot = avgPot
                                   };
                    break;

                case KindGroup.MONTH:
                    group = from Measure in listMeasureOrig
                                     group Measure
                                     by new { Measure.date.Year, Measure.date.Month }
                                into g
                                     let avgCurr = g.Average(a => a.Current)
                                     let avgNapr = g.Average(a => a.Napr)
                                     let avgPot = g.Average(a => a.SummPot)
                                     select new Measure
                                     {
                                         Current = avgCurr,
                                         date = new DateTime(g.Key.Year, g.Key.Month, 1),
                                         Napr = avgNapr,
                                         SummPot = avgPot
                                     };
                    break;

                case KindGroup.YEAR:
                    group = from Measure in listMeasureOrig
                                    group Measure by Measure.date.Year into g
                                    let avgCurr = g.Average(a => a.Current)
                                    let avgNapr = g.Average(a => a.Napr)
                                    let avgPot = g.Average(a => a.SummPot)
                                    select new Measure
                                    {
                                        Current = avgCurr,
                                        date = new DateTime(g.Key, 1, 1),
                                        Napr = avgNapr,
                                        SummPot = avgPot
                                    };
                    break;
            }

            listMeasure = new ObservableCollection<Measure> (group.ToList());
        }

    }
}
