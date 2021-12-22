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
using ResourceAZ.Calculation;
using Microsoft.Win32;
using System.Collections;

namespace ResourceAZ.ViewModels
{

     internal partial class MainWindowViewModel : ViewModel
    {
        public double[] ApproxA;
        public double[] ApproxR;

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
                    dpAavg = CalcApproxLine(ModelA, dpA, ref ApproxA);
                    dpRavg = CalcApproxLine(ModelR, dpR, ref ApproxR);
                }
            }
        }
        public IList _SelectedMeasure;
        public IList SelectedMeasure {
            get => _SelectedMeasure;
            set
            {
                _SelectedMeasure = value;
            }
        }


        // измерения, преобраованные в списки точек для графика
        #region
        public List<DataPoint> dpCurrent { get; set; }
        List<DataPoint> dpNapr { get; set; }
        List<DataPoint> dpPot { get; set; }
        List<DataPoint> dpA { get; set; }
        public List<DataPoint> dpAavg { get; set; }
        List<DataPoint> dpR { get; set; }
        public List<DataPoint> dpRavg { get; set; }


        // ObservableCollection<DataPoint> dpRavg { get; set; }
        // модели для графиков
        public PlotModel ModelCurrent { get; }
        public PlotModel ModelNapr { get; }
        public PlotModel ModelPot { get; }
        public PlotModel ModelA { get; }
        public PlotModel ModelR { get; }
        #endregion

        // база данных измерений
        //private IMeasureData repository = new MeasureDataGen();
        private IMeasureData repository = new MeasureDataExcel();


        public KindGroup SelectGroup;

        // переменные связанные с экраннй формой
        #region
        bool _GroupNone;
        public bool GroupNone
        {
            get => _GroupNone;
            set
            {
                Set(ref _GroupNone, value);
            }
        }
        public bool GroupYear { get; set; }
        public bool GroupMonth { get; set; }
        public bool GroupDay { get; set; }

        public bool RemoveBadValue { get; set; }

        public bool CalcPotencial { get; set; } = true;
        public bool CalcResist { get; set; }
        public double MinPotCalc { get; set; } = -0.9;
        public double MaxCurrentSKZ { get; set; } = 15.0;
        public double MaxNaprSKZ { get; set; } = 48.0;
        private string _FileName;
        public string FileName
        {
            get => _FileName;
            set
            {
                Set(ref _FileName, value);
            }
        }

        #endregion

        #region Команды
        public ICommand CloseApplicationCommand { get; }
        private bool CanCloseApplicationCommand(object p) => true;

        private void OnCloseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }

        public ICommand FromExcelCommand { get; }
        private bool CanFromExcelCommand(object p) => true;
        private void OnFromExcelCommandExecuted(object p)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "Excel (*.xlsx)|*.xlsx";
            od.DefaultExt = "Excel (*.xlsx)|*.xlsx";

            if (od.ShowDialog() != true)
                return;

            FileName = "";

            listMeasureOrig = repository.GetAllData(od.FileName);

            OpenNewList();

            FileName = od.FileName;

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
            return listMeasure.Count > 1;
        }

        private void OnCalculateCommand(object p)
        {
            KindCalc kc = CalcPotencial ? KindCalc.Potencial : KindCalc.Resist;
            double LastR = listMeasure[listMeasure.Count - 1].Resist;
            double LastA = listMeasure[listMeasure.Count - 1].Koeff;

            //if(dpRavg[dpRavg.Count - 1].Y >= dpRavg[0].Y)
            //{
            //    MessageBox.Show("Расчет невозможен. \nСопротивление со временем должно уменьшаться.","Ошибка", MessageBoxButton.OK, MessageBoxImage.Error );
            //    return;
            //}

            CalcWindowViewModel vm = new CalcWindowViewModel(this);
            Window win = (Application.Current as App).displayRootRegistry.CreateWindowWithVM(vm);
            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.Show();

        }

        // команда удаления строки из datagrid
        public ICommand DeleteLineCommand { get; }
        private bool CanDeleteLineCommand(object p)
        {
            //return true;
            return SelectedMeasure != null && SelectedMeasure?.Count > 0;
        }

        private void OnDeleteLineCommand(object p)
        {
            while (SelectedMeasure.Count > 0)
            {
                Measure selMeas = SelectedMeasure[0] as Measure;
                Measure meas = listMeasureOrig.Where(m => m.date == selMeas.date 
                    && m.Resist == selMeas.Resist)
                    .FirstOrDefault();
                listMeasureOrig.Remove(meas);
                listMeasure.Remove(selMeas);
            }

            //foreach (Measure measure in SelectedMeasure)
            //{
            //    //Measure selMeas = SelectedMeasure[0] as Measure;
            //    Measure meas = listMeasureOrig.Where(m => m.date == measure.date /*&& m.Current == selMeas.Current*/
            //        && m.Resist == measure.Resist)
            //        .FirstOrDefault();
            //    listMeasureOrig.Remove(meas);
            //    listMeasure.Remove(measure);
            //}

            ModelToChart(listMeasure);
            dpAavg = CalcApproxLine(ModelA, dpA, ref ApproxA);
            dpRavg = CalcApproxLine(ModelR, dpR, ref ApproxR);

        }

        // команда удаления строки из datagrid
        public ICommand RemoveDeviationCommand { get; }
        private bool CanRemoveDeviationCommand(object p)
        {
            return listMeasureOrig?.Count > 3;
        }

        private void OnRemoveDeviationCommand(object p)
        {
            double AvgSumPot = listMeasureOrig.Average(a => a.SummPot);

            double AvgCurrent = listMeasureOrig.Average(a => a.Current);

            double AvgNapr = listMeasureOrig.Average(a => a.Napr);

            double AvgResist = listMeasureOrig.Average(a => a.Resist);

            double AvgKoef = listMeasureOrig.Average(a => a.Koeff);


            IEnumerable<Measure> list = listMeasureOrig.Where(w => w.SummPot > -0.05 || w.SummPot < AvgSumPot * 2 ||
                    w.Current < 0.01 || w.Current > AvgCurrent * 2 ||
                    w.Napr < 0.01 || w.Napr > AvgNapr * 2 ||
                    w.Resist < 0.1 || w.Resist > 50 ||
                    w.Koeff > -0.1 || w.Koeff < AvgKoef * 2);

            while (list.Count() > 0)
                listMeasureOrig.Remove(list.First());

            FormatListMeasure(SelectGroup);

        }

        #endregion

        public MainWindowViewModel()
        {
            // инициализация команд
            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommand);
            CalculateCommand = new LambdaCommand(OnCalculateCommand, CanCalculateCommand);
            FromExcelCommand = new LambdaCommand(OnFromExcelCommandExecuted, CanFromExcelCommand);
            GroupByCommand = new LambdaCommand(OnGroupByCommandExecuted, CanGroupByCommand);
            DeleteLineCommand = new LambdaCommand(OnDeleteLineCommand, CanDeleteLineCommand);
            RemoveDeviationCommand = new LambdaCommand(OnRemoveDeviationCommand, CanRemoveDeviationCommand);

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
            //listMeasureOrig = repository.GetAllData();

            //OpenNewList();

            listMeasureOrig = new ObservableCollection<Measure>();
            listMeasure = new ObservableCollection<Measure>();

        }


        void OpenNewList()
        {
            GroupNone = true;
            SelectGroup = KindGroup.NONE;

            foreach (Measure m in listMeasureOrig)
            {
                m.Koeff = m.SummPot / m.Current;
                m.Resist = m.Napr / m.Current + m.Koeff;
            }

            listMeasure = new ObservableCollection<Measure>(listMeasureOrig);

            // отмечаем на экране первый RadioButton
            GroupNone = true;

            dpAavg = CalcApproxLine(ModelA, dpA, ref ApproxA);
            dpRavg = CalcApproxLine(ModelR, dpR, ref ApproxR);

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
                                Resist = avgNapr / avgCurr + (avgPot / avgCurr)
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
                                         Resist = avgNapr / avgCurr + (avgPot / avgCurr)
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
                                date = new DateTime(g.Key, 7, 1),
                                Napr = avgNapr,
                                SummPot = avgPot,
                                Koeff = avgPot / avgCurr,
                                Resist = avgNapr / avgCurr + (avgPot / avgCurr)
                            };
                    break;

                case KindGroup.SUMMER:
                    group = from Measure in listMeasureOrig
                            where Measure.date.Month < 9 && Measure.date.Month > 5
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
                                Resist = avgNapr / avgCurr + (avgPot / avgCurr)
                            };
                    break;
            }

            listMeasure = new ObservableCollection<Measure> (group.ToList());

            dpAavg = CalcApproxLine(ModelA, dpA, ref ApproxA);
            dpRavg = CalcApproxLine(ModelR, dpR, ref ApproxR);

        }

    }
}
