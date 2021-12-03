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

     internal partial class MainWindowViewModel : ViewModel
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
                if (CalcPotencial)
                {
                    CalcApproxLine(ModelA, dpA);
                    CalcApproxLine(ModelR, dpR);
                }
            }
        }

        // измерения, преобраованные в списки точек для графика
        #region
        public ObservableCollection<DataPoint> dpCurrent { get; set; }
        ObservableCollection<DataPoint> dpNapr { get; set; }
        ObservableCollection<DataPoint> dpPot { get; set; }
        ObservableCollection<DataPoint> dpA { get; set; }
        //ObservableCollection<DataPoint> dpAavg { get; set; }
        ObservableCollection<DataPoint> dpR { get; set; }
       // ObservableCollection<DataPoint> dpRavg { get; set; }
        // модели для графиков
        public PlotModel ModelCurrent { get; }
        public PlotModel ModelNapr { get; }
        public PlotModel ModelPot { get; }
        public PlotModel ModelA { get; }
        public PlotModel ModelR { get; }
        #endregion

        // база данных измерений
        private IMeasureData repository = new MeasureDataGen();


        public KindGroup SelectGroup;

        // переменные связанные с экраннй формой
        #region
        public bool GroupNone { get; set; }
        public bool GroupYear { get; set; }
        public bool GroupMonth { get; set; }
        public bool GroupDay { get; set; }

        public bool RemoveBadValue { get; set; }

        public bool CalcPotencial { get; set; } = true;
        public bool CalcResist { get; set; }
        public double MinPotCalc { get; set; } = -0.9;
        public double MaxCurrentSKZ { get; set; } = 15.0;
        public double MaxNaprSKZ { get; set; } = 48.0;
        #endregion

        #region Команды
        public ICommand CloseApplicationCommand { get; }
        private bool CanCloseApplicationCommand(object p) => true;

        private void OnCloseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }

        public ICommand KindCalcCommand { get; }
        private bool CanKindCalcCommand(object p) => true;
        private void OnKindCalcCommandCommandExecuted(object p)
        {
            //KindCalc param = (KindCalc)p;
            //CalcApproxLine(ModelA, dpA);
            //CalcApproxLine(ModelR, dpR);
            //if (param == KindCalc.ApprLine)
            //{
            //    CalcApproxLine(ModelA, dpA);
            //    CalcApproxLine(ModelR, dpR);
            //}
            //else
            //{
            //    if (ModelA.Series.Count == 2)
            //    {
            //        ModelA.Series[1].IsVisible = false;
            //        ModelA.InvalidatePlot(true);
            //    }

            //    if (ModelR.Series.Count == 2)
            //    {
            //        ModelR.Series[1].IsVisible = false;
            //        ModelR.InvalidatePlot(true);
            //    }
            //}
        }

        public ICommand GroupByCommand { get; }
        private bool CanGroupByCommand(object p) => true;
        private void OnGroupByCommandExecuted(object p)
        {
            KindGroup param = (KindGroup)p;
            FormatListMeasure(param);
            SelectGroup = param;

        }

        // команда на расчеты
        public ICommand CalculateCommand { get; }
        private bool CanCalculateCommand(object p)
        {
            return listMeasure.Count > 2;
        }

        private void OnCalculateCommand(object p)
        {
            KindCalc kc = CalcPotencial ? KindCalc.Potencial : KindCalc.Resist;
            double LastR = listMeasure[listMeasure.Count - 1].Resist;
            double LastA = listMeasure[listMeasure.Count - 1].Koeff;

            //CalcWindowViewModel vm = new CalcWindowViewModel(SelectGroup, kc, MinPotCalc, listMeasure);
            CalcWindowViewModel vm = new CalcWindowViewModel(this);
            Window win = (Application.Current as App).displayRootRegistry.CreateWindowWithVM(vm);
            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.Show();

        }

        #endregion

        public MainWindowViewModel()
        {
            // инициализация команд
            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommand);
            CalculateCommand = new LambdaCommand(OnCalculateCommand, CanCalculateCommand);
            KindCalcCommand = new LambdaCommand(OnKindCalcCommandCommandExecuted, CanKindCalcCommand);
            GroupByCommand = new LambdaCommand(OnGroupByCommandExecuted, CanGroupByCommand);

            // подгтовка графиков для всех измерений
            ModelCurrent = new PlotModel();
            ModelNapr = new PlotModel();
            ModelPot = new PlotModel();
            ModelA = new PlotModel();
            ModelR = new PlotModel();

            dpCurrent = InitChart(ModelCurrent, "Выходной ток");
            dpNapr = InitChart(ModelNapr, "Напряжение");
            dpPot = InitChart(ModelPot, "Потенциал");
            dpA = InitChart(ModelA, "Коэффициенты");
            dpR = InitChart(ModelR, "Сопротивление");

            // получение списка значений
            listMeasureOrig = repository.GetAllData();
            foreach(Measure m in listMeasureOrig)
            {
                m.Koeff = m.SummPot / m.Current;
                m.Resist = m.Napr / m.Current;
            }

            listMeasure = new ObservableCollection<Measure>(listMeasureOrig);

            // отмечаем на экране первый RadioButton
            GroupNone = true;

            CalcApproxLine(ModelA, dpA);
            CalcApproxLine(ModelR, dpR);

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
                                       SummPot = avgPot,
                                       Koeff = avgPot / avgCurr,
                                       Resist = avgNapr / avgCurr
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
                                         SummPot = avgPot,
                                         Koeff = avgPot / avgCurr,
                                         Resist = avgNapr / avgCurr
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
                                SummPot = avgPot,
                                Koeff = avgPot / avgCurr,
                                Resist = avgNapr / avgCurr
                            };
                    break;
            }

            listMeasure = new ObservableCollection<Measure> (group.ToList());
        }

    }
}
