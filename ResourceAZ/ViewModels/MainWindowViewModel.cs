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
using ResourceAZ.Chart;

namespace ResourceAZ.ViewModels
{

     internal partial class MainWindowViewModel : ViewModel
    {
        public double[] ApproxA;
        public double[] ApproxR;

        //public int EndRange;
        DateTime _MinSelectedValue;
        public DateTime MinSelectedValue
        {
            get => _MinSelectedValue; set { Set(ref _MinSelectedValue, value); }
        }
        DateTime _MaxSelectedValue;
        public DateTime MaxSelectedValue
        {
            get => _MaxSelectedValue; set { Set(ref _MaxSelectedValue, value); }
        }

        public bool RangeForCalc;

        private string _KoeffFunc;
        public string KoeffFunc
        {
            get => _KoeffFunc; set { Set(ref _KoeffFunc, value); }
        }
        private string _ResistFunc;
        public string ResistFunc
        {
            get => _ResistFunc; set { Set(ref _ResistFunc, value); }
        }

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
                //if (CalcPotencial)
                //{
                //    dpAavg = CalcApproxLine(ModelA, dpA, KindLineApprox.KOEFF);
                //    dpRavg = CalcApproxLine(ModelR, dpR, KindLineApprox.RESIST);
                //}
            }
        }
        public IList _SelectedMeasure;
        public IList SelectedMeasure
        {
            get => _SelectedMeasure;
            set
            {
                Set(ref _SelectedMeasure, value);
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
        public MyPlotModel ModelCurrent { get; }
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
        public int orderCalc { get; set; } = 4;

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
        bool _GroupDay;
        public bool GroupDay
        {
            get => _GroupDay; set { Set(ref _GroupDay, value); }
        }

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

            CalcWindowViewModel vm = new CalcWindowViewModel(this);
            Window win = (Application.Current as App).displayRootRegistry.CreateWindowWithVM(vm);
            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.Show();

        }

        // команда удаления строки из datagrid
        public ICommand DeleteLineCommand { get; }
        private bool CanDeleteLineCommand(object p)
        {
            return SelectedMeasure != null && SelectedMeasure?.Count > 0;
        }
        // команда удаления строки из datagrid
        private void OnDeleteLineCommand(object p)
        {
            RemoveMeasureFromOrig(SelectedMeasure, SelectGroup);

            while (SelectedMeasure.Count > 0)
            {
                listMeasure.Remove((Measure)SelectedMeasure[0]);
            }

            ModelToChart(listMeasure);
            dpAavg = CalcApproxLine(ModelA, dpA, KindLineApprox.KOEFF);
            dpRavg = CalcApproxLine(ModelR, dpR, KindLineApprox.RESIST);

        }

        // команда на удаление недостоверных показаний
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

        // команда удаления выбранного диапазона
        public ICommand RemoveSelectedValuesCommand { get; }
        private bool CanRemoveSelectedValuesCommand(object p)
        {
            return listMeasure.Where(w => w.SetColor).Count() > 0;
        }
        private void OnRemoveSelectedValuesCommand(object p)
        {

            IList list = listMeasure.Where(w => w.SetColor).ToList();

            RemoveMeasureFromOrig(list, SelectGroup);

            foreach(Measure m in list)
                listMeasure.Remove(m);

            ModelToChart(listMeasure);

            OnDropRangeCommand(p);
        }

        // комнда принятия диапазона для равсчетов
        public ICommand SetRangeForCalcCommand { get; }
        private bool CanSetRangeForCalcCommand(object p)
        {
            return listMeasure.Where(w => w.SetColor).Count() > 0;
        }
        private void OnSetRangeForCalcCommand(object p)
        {

            RangeForCalc = true;

            double minDate = MinSelectedValue.ToOADate();
            double maxDate = MaxSelectedValue.ToOADate();

            List<DataPoint> RangeA = dpA.Where(w => w.X >= minDate && w.X <= maxDate).ToList();
            dpAavg = CalcApproxLine(ModelA, RangeA, KindLineApprox.KOEFF, dpA[dpA.Count-1].X, 10);

            List<DataPoint> RangeR = dpR.Where(w => w.X >= minDate && w.X <= maxDate).ToList();
            dpRavg = CalcApproxLine(ModelR, RangeR, KindLineApprox.RESIST, dpR[dpR.Count - 1].X, 10);

            // обновление точек графиков на экране для смены цвета фона
            ModelCurrent.InvalidatePlot(true);
            ModelNapr.InvalidatePlot(true);
            ModelPot.InvalidatePlot(true);
        }

        // команда снятия выделения
        public ICommand DropRangeCommand { get; }
        private bool CanDropRangeCalcCommand(object p)
        {
            return MinSelectedValue < MaxSelectedValue;
        }
        private void OnDropRangeCommand(object p)
        {

            RangeForCalc = false;
            MinSelectedValue = DateTime.MinValue;
            MaxSelectedValue = DateTime.MinValue;

            foreach (Measure m in listMeasure)
                m.SetColor = false;

            dpAavg = CalcApproxLine(ModelA, dpA, KindLineApprox.KOEFF);
            dpRavg = CalcApproxLine(ModelR, dpR, KindLineApprox.RESIST);

            // обновление точек графиков на экране для смены цвета фона
            ModelCurrent.InvalidatePlot(true);
            ModelNapr.InvalidatePlot(true);
            ModelPot.InvalidatePlot(true);
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
            RemoveSelectedValuesCommand = new LambdaCommand(OnRemoveSelectedValuesCommand, CanRemoveSelectedValuesCommand);
            SetRangeForCalcCommand = new LambdaCommand(OnSetRangeForCalcCommand, CanSetRangeForCalcCommand);
            DropRangeCommand = new LambdaCommand(OnDropRangeCommand, CanDropRangeCalcCommand);

            // подгтовка графиков для всех измерений
            ModelCurrent = new MyPlotModel(this);
            ModelNapr = new MyPlotModel(this);
            ModelPot = new MyPlotModel(this);
            ModelA = new MyPlotModel(this);
            ModelR = new MyPlotModel(this);

            dpCurrent = InitChart(ModelCurrent, "Выходной ток");
            dpNapr = InitChart(ModelNapr, "Напряжение");
            dpPot = InitChart(ModelPot, "Потенциал");
            dpA = InitChart(ModelA, "Коэффициенты");
            dpR = InitChart(ModelR, "Сопротивление");

            listMeasureOrig = new ObservableCollection<Measure>();
            listMeasure = new ObservableCollection<Measure>();

        }


        void OpenNewList()
        {
            //GroupNone = true;
            SelectGroup = KindGroup.DAY;

            ModelR.DefaultYAxis.Maximum = double.NaN;
            ModelR.DefaultYAxis.Minimum = double.NaN;
            ModelA.DefaultYAxis.Maximum = double.NaN;
            ModelA.DefaultYAxis.Minimum = double.NaN;

            ModelCurrent.ResetAllAxes();
            ModelNapr.ResetAllAxes();
            ModelPot.ResetAllAxes();
            ModelA.ResetAllAxes();
            ModelR.ResetAllAxes();

            MinSelectedValue = DateTime.MinValue;
            MaxSelectedValue = DateTime.MinValue;

            foreach (Measure m in listMeasureOrig)
            {
                m.Koeff = m.SummPot / m.Current;
                m.Resist = m.Napr / m.Current;// + m.Koeff;
            }

            //listMeasure = new ObservableCollection<Measure>(listMeasureOrig);
            FormatListMeasure(SelectGroup);

            // отмечаем на экране первый RadioButton
            GroupDay = true;

            dpAavg = CalcApproxLine(ModelA, dpA, KindLineApprox.KOEFF);
            dpRavg = CalcApproxLine(ModelR, dpR, KindLineApprox.RESIST);
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
                                Resist = avgNapr / avgCurr// + (avgPot / avgCurr)
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
                                Resist = avgNapr / avgCurr// + (avgPot / avgCurr)
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
                                Resist = avgNapr / avgCurr// + (avgPot / avgCurr)
                            };
                    break;

                //case KindGroup.SUMMER:
                //    group = from Measure in listMeasureOrig
                //            where Measure.date.Month < 9 && Measure.date.Month > 5
                //            group Measure
                //            by new { Measure.date.Year, Measure.date.Month, Measure.date.Day }
                //                into g
                //            let avgCurr = g.Average(a => a.Current)
                //            let avgNapr = g.Average(a => a.Napr)
                //            let avgPot = g.Average(a => a.SummPot)
                //            select new Measure
                //            {
                //                Current = avgCurr,
                //                date = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                //                Napr = avgNapr,
                //                SummPot = avgPot,
                //                Koeff = avgPot / avgCurr,
                //                Resist = avgNapr / avgCurr// + (avgPot / avgCurr)
                //            };
                //    break;
            }

            listMeasure = new ObservableCollection<Measure> (group.ToList());

        }

        // отметка цветом для выбранного времени
        public void SelectRangeDataGrid(DateTime dtFrom, DateTime dtTo)
        {
            RangeForCalc = false;
            //MinSelectedValue = DateTime.MinValue;
            //MaxSelectedValue = DateTime.MinValue;

            foreach (Measure m in listMeasure)
                m.SetColor = m.date <= dtTo && m.date >= dtFrom;

            // обновление точек графиков на экране
            ModelCurrent.InvalidatePlot(true);
            ModelNapr.InvalidatePlot(true);
            ModelPot.InvalidatePlot(true);
            ModelA.InvalidatePlot(true);
            ModelR.InvalidatePlot(true);

        }


        // удаление выбранных дат из оригинального списка
        private void RemoveMeasureFromOrig(IList list, KindGroup kind)
        {
            IEnumerable<Measure> meas;

            foreach (Measure selMeas in list)
            {
                switch(kind)
                {
                    case KindGroup.DAY:
                        meas = listMeasureOrig.Where(m => m.date.Year == selMeas.date.Year 
                                && m.date.Month == selMeas.date.Month
                                && m.date.Day == selMeas.date.Day);
                        break;

                    case KindGroup.MONTH:
                        meas = listMeasureOrig.Where(m => m.date.Year == selMeas.date.Year
                                && m.date.Month == selMeas.date.Month);
                        break;

                    case KindGroup.YEAR:
                        meas = listMeasureOrig.Where(m => m.date.Year == selMeas.date.Year);
                        break;

                    default:
                        meas = listMeasureOrig.Where(m => m.date == selMeas.date  && m.Resist == selMeas.Resist);
                        break;

                }

                while(meas.Count() > 0)
                    listMeasureOrig.Remove(meas.First());
            }

        }

    }
}
