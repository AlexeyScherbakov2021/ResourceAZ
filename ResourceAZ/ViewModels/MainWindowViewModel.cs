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
using System.Windows.Media;
using OxyPlot.Axes;
using System.Collections.ObjectModel;
using ResourceAZ.Views;
using ResourceAZ.Calculation;
using Microsoft.Win32;
using System.Collections;
using ResourceAZ.Chart;
using ResourceAZ.ScottChart;
using ScottPlot;

namespace ResourceAZ.ViewModels
{

    internal partial class MainWindowViewModel : ViewModel
    {
        //public double[] ApproxA;
        //public double[] ApproxR;

        public ApproxLine ApproxA;
        public ApproxLine ApproxR;

        public List<scottChart> listPlot;
        public int indexX1 = -1;
        public int indexX2 = -1;

        private double _X1 = double.NaN;
        public double X1
        {
            get => _X1;
            set
            {
                _X1 = value;
                if(!double.IsNaN(_X1))
                    indexX1 = Array.IndexOf(dates, dates.FirstOrDefault(n => n >= _X1));
                else
                    indexX1 = 0;
            }
        }


        private double _X2 = double.NaN;
        public double X2
        {
            get => _X2;
            set
            {
                _X2 = value;
                if (!double.IsNaN(_X2))
                    indexX2 = Array.IndexOf(dates, dates.LastOrDefault(n => n <= _X2));
                else
                    indexX2 = dates.Length - 1;
            }
        }

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
                if(_listMeasure.Count > 1)
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
        public double[] dates;
        public double[] currents;
        public double[] naprs;
        public double[] sumpots;
        public double[] koeffs;
        public double[] resists;

        public double[] Aavg;
        public double[] Ravg;


        // модели для графиков
        scottChart chartCurrent;
        scottChart chartNapr;
        scottChart chartPot;
        scottChart chartKoeff;
        scottChart chartResist;
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
        private string _FileName = "Расчет ресурса Анодного заземлителя";
        public string FileName
        {
            get => _FileName;
            set
            {
                Set(ref _FileName, "Расчет ресурса Анодного заземлителя : " + value);
            }
        }
        public bool _SetSelectedRange;
        public bool SetSelectedRange { get => _SetSelectedRange; set { Set(ref _SetSelectedRange, value); } }

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

            CalculateApproximate();

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

            //OnDropRangeCommand(p);
        }

        // комнда принятия диапазона для равсчетов
        public ICommand SetRangeForCalcCommand { get; }
        private bool CanSetRangeForCalcCommand(object p)
        {
            //return listMeasure.Where(w => w.SetColor).Count() > 0;
            return listMeasure.Count() > 0;
        }
        private void OnSetRangeForCalcCommand(object p)
        {

            RangeForCalc = SetSelectedRange;

            //double minDate = MinSelectedValue.ToOADate();
            //double maxDate = MaxSelectedValue.ToOADate();

            if (double.IsNaN(X1))
            {
                X1 = dates[0] + (dates[dates.Length - 1] - dates[0]) / 3;
                X2 = X1 + (dates[dates.Length - 1] - dates[0]) / 3;
            }
            chartCurrent.SetSelectedRange(SetSelectedRange, X1, X2);
        }

        // команда перерасчета линии аппроксимации
        public ICommand DropRangeCommand { get; }
        private bool CanDropRangeCalcCommand(object p)
        {
            return listMeasure.Count() > 0;
        }
        private void OnDropRangeCommand(object p)
        {
            if(RangeForCalc)
            {
                double[] rangeKoeff = new double[indexX2 - indexX1 + 1];
                for (int i = 0, n = indexX1; i < rangeKoeff.Length; i++, n++)
                    rangeKoeff[i] = koeffs[n];

                double[] rangeResist = new double[indexX2 - indexX1 + 1];
                for (int i = 0, n = indexX1; i < rangeResist.Length; i++, n++)
                    rangeResist[i] = resists[n];

                double[] datesRange = new double[indexX2 - indexX1 + 1];
                for (int i = 0, n = indexX1; i < datesRange.Length; i++, n++)
                    datesRange[i] = dates[n];


                Aavg = ApproxA.CalcDataPoint(rangeKoeff, datesRange, orderCalc);
                if (ApproxA.CalcAddRange(dates.Last()))
                {
                    Aavg = ApproxA.GetY();
                    double[] d = ApproxA.GetX();
                    chartKoeff.AddSeriesOrUpdateApprox(d, Aavg);
                }

                //chartKoeff.AddSeriesOrUpdateApprox(dates, Aavg);

                Ravg = ApproxR.CalcDataPoint(rangeResist, datesRange, orderCalc);
                if (ApproxR.CalcAddRange(dates.Last()))
                {
                    Ravg = ApproxR.GetY();
                    double[] d = ApproxR.GetX();
                    chartResist.AddSeriesOrUpdateApprox(d, Ravg);
                    ResistFunc = ApproxR.GetStringFunction();
                }

                //Aavg = CalcApproxLine(chartKoeff, rangeKoeff, datesRange, KindLineApprox.KOEFF, dates[dates.Length - 1]);
                //Ravg = CalcApproxLine(chartResist, rangeResist, datesRange, KindLineApprox.RESIST, dates[dates.Length - 1]);
            }
            else
            {
                CalculateApproximate();

                //Aavg = ApproxA.CalcDataPoint(koeffs, dates, orderCalc);
                //chartKoeff.AddSeriesOrUpdateApprox(dates, Aavg);

                //Ravg = ApproxR.CalcDataPoint(resists, dates, orderCalc);
                //chartResist.AddSeriesOrUpdateApprox(dates, Ravg);
            }

        }


        // событие после загрузки главного окна
        public ICommand CommandLoaded { get; }
        private bool CanCommandLoadedCommand(object p) => true;

        private void OnCommandLoadedCommand(object p)
        {
            chartCurrent = new scottChart(App.mainWindow.PlotCurrent, this);
            chartNapr = new scottChart(App.mainWindow.PlotNapr, this);
            chartPot = new scottChart(App.mainWindow.PlotPot, this);
            chartKoeff = new scottChart(App.mainWindow.PlotKoeff, this);
            chartResist = new scottChart(App.mainWindow.PlotResist, this);
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
            CommandLoaded = new LambdaCommand(OnCommandLoadedCommand, CanCommandLoadedCommand);

            // подгтовка графиков для всех измерений
            ApproxA = new ApproxLine();
            ApproxR = new ApproxLine();

            listPlot = new List<scottChart>();

            dates = new double[0];
            currents = new double[0];

            listMeasureOrig = new ObservableCollection<Measure>();
            listMeasure = new ObservableCollection<Measure>();

        }


        void OpenNewList()
        {
            RangeForCalc = false;
            SelectGroup = KindGroup.DAY;
            X1 = X2 = double.NaN;

            SetSelectedRange = false;
            chartCurrent.SetSelectedRange(false);

            MinSelectedValue = DateTime.MinValue;
            MaxSelectedValue = DateTime.MinValue;

            foreach (Measure m in listMeasureOrig)
            {
                m.Koeff = m.SummPot / m.Current;
                m.Resist = m.Napr / m.Current;// + m.Koeff;
            }

            FormatListMeasure(SelectGroup);

            // отмечаем на экране первый RadioButton
            GroupDay = true;

            CalculateApproximate();

            //Aavg = ApproxA.CalcDataPoint(koeffs, dates, orderCalc);
            //chartKoeff.AddSeriesOrUpdateApprox(dates, Aavg);

            //Ravg = ApproxR.CalcDataPoint(resists, dates);
            //chartResist.AddSeriesOrUpdateApprox(dates, Ravg);

            //Aavg = CalcApproxLine(chartKoeff, koeffs, dates, KindLineApprox.KOEFF);
            //Ravg = CalcApproxLine(chartResist, resists, dates, KindLineApprox.RESIST);
        }


        //--------------------------------------------------------------------------------------------
        // формирование списка по критериям
        //--------------------------------------------------------------------------------------------
        private void FormatListMeasure(KindGroup kind)
        {
            IEnumerable<Measure> group = null;

            switch (kind)
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
            MinSelectedValue = DateTime.MinValue;
            MaxSelectedValue = DateTime.MinValue;

            foreach (Measure m in listMeasure)
                m.SetColor = m.date <= dtTo && m.date >= dtFrom;

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

        void CalculateApproximate()
        {
            Aavg = ApproxA.CalcDataPoint(koeffs, dates, orderCalc);
            chartKoeff.AddSeriesOrUpdateApprox(dates, Aavg);

            Ravg = ApproxR.CalcDataPoint(resists, dates, orderCalc);
            chartResist.AddSeriesOrUpdateApprox(dates, Ravg);

            ResistFunc = ApproxR.GetStringFunction();
        }
    }
}
