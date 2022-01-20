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
        //public ApproxLine ApproxA;
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
                if (!double.IsNaN(_X1))
                {
                    indexX1 = Array.IndexOf(dates, dates.FirstOrDefault(n => n >= _X1));
                    MinSelectedValue = DateTime.FromOADate(_X1);
                }
                else
                {
                    indexX1 = 0;
                    MinSelectedValue = DateTime.MinValue;
                }
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
                {
                    indexX2 = Array.IndexOf(dates, dates.LastOrDefault(n => n <= _X2));
                    MaxSelectedValue = DateTime.FromOADate(_X2);
                }
                else
                {
                    indexX2 = dates.Length - 1;
                    MaxSelectedValue = DateTime.MinValue;
                }
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

        //public bool RangeForCalc { get; set; }

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

        // база данных измерений
        //private IMeasureData repository = new MeasureDataGen();
        private IMeasureData repository = new MeasureDataExcel();

        public KindGroup SelectGroup;


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

        // переменные связанные с экраннй формой
        #region

        public double MinCurrentRemove { get; set; } = 0.4;
        public double MaxCurrentRemove { get; set; } = 80;
        public double MinNaprRemove { get; set; } = 1;
        public double MaxNaprRemove { get; set; } = 100;
        public double MinPotRemove { get; set; } = -6;
        public double MaxPotRemove { get; set; } = 2;
        public double MinResistRemove { get; set; } = 0.3;
        public double MaxResistRemove { get; set; } = 90;

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
        public bool CalcPotencial { get; set; }
        public bool CalcResist { get; set; } = true;
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
        private double? _AvgCurrent;
        public double? AvgCurrent
        {
            get => _AvgCurrent;
            set => Set(ref _AvgCurrent, value);
        }
        #endregion

        #region Команды
        //----------------------------------------------------------------------------------------------
        // команда закрытия программы
        //----------------------------------------------------------------------------------------------
        public ICommand CloseApplicationCommand { get; }
        private bool CanCloseApplicationCommand(object p) => true;
        private void OnCloseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }

        //----------------------------------------------------------------------------------------------
        // команда загрузки из Excel
        //----------------------------------------------------------------------------------------------
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
            if(listMeasureOrig.Count <= 0)
            {
                MessageBox.Show("Не удалось загрузить данные из файла " + od.FileName, "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            OpenNewList();

            FileName = od.FileName;

        }

        //----------------------------------------------------------------------------------------------
        // команада группировки измерений
        //----------------------------------------------------------------------------------------------
        public ICommand GroupByCommand { get; }
        private bool CanGroupByCommand(object p) => true;
        private void OnGroupByCommandExecuted(object p)
        {
            KindGroup param = (KindGroup)p;
            FormatListMeasure(param);
            SelectGroup = param;
        }

        //----------------------------------------------------------------------------------------------
        // команда на расчеты
        //----------------------------------------------------------------------------------------------
        public ICommand CalculateCommand { get; }
        private bool CanCalculateCommand(object p)
        {
            return listMeasure.Count > 1;
        }
        private void OnCalculateCommand(object p)
        {
            // предварительный равсет средних линий
            OnCalcApproxCommand(p);
            // получение типа расчета
            KindCalc kc = CalcPotencial ? KindCalc.Potencial : KindCalc.Resist;
            // последние значения сопротивления и коэффициента
            double LastR = listMeasure[listMeasure.Count - 1].Resist;
            double LastA = listMeasure[listMeasure.Count - 1].Koeff;

            // открыть окно расчета
            CalcWindowViewModel vm = new CalcWindowViewModel(this);
            Window win = (Application.Current as App).displayRootRegistry.CreateWindowWithVM(vm);
            vm.view = (CalcWindow)win;
            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.Show();

        }

        //----------------------------------------------------------------------------------------------
        // команда удаления строки из datagrid
        //----------------------------------------------------------------------------------------------
        public ICommand DeleteLineCommand { get; }
        private bool CanDeleteLineCommand(object p)
        {
            return SelectedMeasure != null && SelectedMeasure?.Count > 0;
        }

        //----------------------------------------------------------------------------------------------
        // команда удаления строк из datagrid
        //----------------------------------------------------------------------------------------------
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

        //----------------------------------------------------------------------------------------------
        // команда на удаление недостоверных показаний
        //----------------------------------------------------------------------------------------------
        public ICommand RemoveDeviationCommand { get; }
        private bool CanRemoveDeviationCommand(object p)
        {
            return listMeasureOrig?.Count > 3;
        }
        private void OnRemoveDeviationCommand(object p)
        {
            //double AvgSumPot = listMeasureOrig.Average(a => a.SummPot);
            //double AvgCurrent = listMeasureOrig.Average(a => a.Current);
            //double AvgNapr = listMeasureOrig.Average(a => a.Napr);
            //double AvgResist = listMeasureOrig.Average(a => a.Resist);
            //double AvgKoef = listMeasureOrig.Average(a => a.Koeff);

            Mouse.OverrideCursor = Cursors.Wait;

            IEnumerable<Measure> list = listMeasureOrig.Where(w => w.SummPot > MaxPotRemove || w.SummPot < MinPotRemove 
                    || w.Current < MinCurrentRemove || w.Current > MaxCurrentRemove 
                    || w.Napr < MinNaprRemove || w.Napr > MaxNaprRemove 
                    || w.Resist < MinResistRemove || w.Resist > MaxResistRemove 
                    );

            while (list.Count() > 0)
                listMeasureOrig.Remove(list.First());

            FormatListMeasure(SelectGroup);

            Mouse.OverrideCursor = null;
        }

        //----------------------------------------------------------------------------------------------
        // команда удаления выбранного диапазона
        //----------------------------------------------------------------------------------------------
        public ICommand RemoveSelectedValuesCommand { get; }
        private bool CanRemoveSelectedValuesCommand(object p)
        {
            return listMeasure.Where(w => w.SetColor).Count() > 0;
        }
        private void OnRemoveSelectedValuesCommand(object p)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            IList list = listMeasure.Where(w => w.SetColor).ToList();

            RemoveMeasureFromOrig(list, SelectGroup);

            foreach(Measure m in list)
                listMeasure.Remove(m);

            ModelToChart(listMeasure);

            // переасчет средних линий

            CalculateApproximate();

            Mouse.OverrideCursor = null;

            //OnDropRangeCommand(p);
        }

        //----------------------------------------------------------------------------------------------
        // комнда установки-снятия диапазона
        //----------------------------------------------------------------------------------------------
        public ICommand SetRangeForCalcCommand { get; }
        private bool CanSetRangeForCalcCommand(object p)
        {
            return listMeasure.Count() > 0;
        }
        private void OnSetRangeForCalcCommand(object p)
        {

            //RangeForCalc = SetSelectedRange;

            if (SetSelectedRange)
            {
                X1 = dates.First();
                X2 = dates.Last();
                //X1 = dates[0] + (dates.Last() - dates.First()) / 3;
                //X2 = X1 + (dates.Last() - dates.First()) / 3;
            }
            else
            {
                X1 = double.NaN;
                X2 = double.NaN;
            }
            chartCurrent.SetSelectedRange(SetSelectedRange, X1, X2);
            SelectRangeDataGrid(MinSelectedValue, MaxSelectedValue);
            OnCalcApproxCommand(p);
        }

        //----------------------------------------------------------------------------------------------
        // команда перерасчета линии аппроксимации
        //----------------------------------------------------------------------------------------------
        public ICommand CalcApproxCommand { get; }
        private bool CanCalcApproxCommand(object p)
        {
            return listMeasure.Count() > 0;
        }
        private void OnCalcApproxCommand(object p)
        {
            if(SetSelectedRange)
            {
                // если был выбран диапазон

                // получаем точки, попавшие в диапазон
                double[] rangeKoeff = new double[indexX2 - indexX1 + 1];
                for (int i = 0, n = indexX1; i < rangeKoeff.Length; i++, n++)
                    rangeKoeff[i] = koeffs[n];

                double[] rangeResist = new double[indexX2 - indexX1 + 1];
                for (int i = 0, n = indexX1; i < rangeResist.Length; i++, n++)
                    rangeResist[i] = resists[n];

                double[] datesRange = new double[indexX2 - indexX1 + 1];
                for (int i = 0, n = indexX1; i < datesRange.Length; i++, n++)
                    datesRange[i] = dates[n];

                // рассчитываем среднюю линию коэффициентов для диапазона 
                //Aavg = ApproxA.CalcDataPoint(rangeKoeff, datesRange, orderCalc);
                // добавление расчетных точек
                //if (ApproxA.CalcAddRange(dates.Last()))
                //{
                //    // если добавленные точки есть, добавляем их в график
                //    Aavg = ApproxA.GetY();
                //    double[] d = ApproxA.GetX();
                //    chartKoeff.AddSeriesOrUpdateApprox(d, Aavg);
                //}

                // рассчитываем среднюю линию сопротивлений для диапазона 
                Ravg = ApproxR.CalcDataPoint(rangeResist, datesRange, orderCalc);
                // добавление расчетных точек
                if (ApproxR.CalcAddRange(dates.Last()))
                {
                    // если добавленные точки есть, добавляем их в график
                    Ravg = ApproxR.GetY();
                    double[] d = ApproxR.GetX();
                    chartResist.AddSeriesOrUpdateApprox(d, Ravg);
                    // получение рассчитанной функции в виде строки
                    ResistFunc = ApproxR.GetStringFunction();
                }

            }
            else
            {
                CalculateApproximate();
            }

        }

        //----------------------------------------------------------------------------------------------
        // событие после загрузки главного окна
        //----------------------------------------------------------------------------------------------
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
            CommandLoaded = new LambdaCommand(OnCommandLoadedCommand, CanCommandLoadedCommand);
            GroupByCommand = new LambdaCommand(OnGroupByCommandExecuted, CanGroupByCommand);
            CalculateCommand = new LambdaCommand(OnCalculateCommand, CanCalculateCommand);
            FromExcelCommand = new LambdaCommand(OnFromExcelCommandExecuted, CanFromExcelCommand);
            DeleteLineCommand = new LambdaCommand(OnDeleteLineCommand, CanDeleteLineCommand);
            CalcApproxCommand = new LambdaCommand(OnCalcApproxCommand, CanCalcApproxCommand);
            RemoveDeviationCommand = new LambdaCommand(OnRemoveDeviationCommand, CanRemoveDeviationCommand);
            SetRangeForCalcCommand = new LambdaCommand(OnSetRangeForCalcCommand, CanSetRangeForCalcCommand);
            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommand);
            RemoveSelectedValuesCommand = new LambdaCommand(OnRemoveSelectedValuesCommand, CanRemoveSelectedValuesCommand);

            // подгтовка графиков для всех измерений
            //ApproxA = new ApproxLine();
            ApproxR = new ApproxLine();

            listPlot = new List<scottChart>();

            dates = new double[0];
            currents = new double[0];



            listMeasureOrig = new ObservableCollection<Measure>();
            listMeasure = new ObservableCollection<Measure>();

        }


        void OpenNewList()
        {
            //SetSelectedRange = false;
            SelectGroup = KindGroup.DAY;

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
            X1 = listMeasure.First().date.ToOADate();
            X2 = listMeasure.Last().date.ToOADate();

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
                    Mouse.OverrideCursor = Cursors.Wait;
                    listMeasure = new ObservableCollection<Measure>(listMeasureOrig);
                    Mouse.OverrideCursor = null;
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
            CalculateApproximate();

        }

        //----------------------------------------------------------------------------------------------
        // отметка цветом для выбранного времени
        //----------------------------------------------------------------------------------------------
        public void SelectRangeDataGrid(DateTime dtFrom, DateTime dtTo)
        {
            //SetSelectedRange = false;
            //MinSelectedValue = DateTime.MinValue;
            //MaxSelectedValue = DateTime.MinValue;
            //MinSelectedValue = dtFrom;
            //MaxSelectedValue = dtTo;

            foreach (Measure m in listMeasure)
                m.SetColor = m.date <= dtTo && m.date >= dtFrom;

        }


        //----------------------------------------------------------------------------------------------
        // удаление выбранных дат из оригинального списка
        //----------------------------------------------------------------------------------------------
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


        //----------------------------------------------------------------------------------------------
        // расчет средних линий для сопротивления и коэффициентов
        //----------------------------------------------------------------------------------------------
        void CalculateApproximate()
        {
            //Aavg = ApproxA.CalcDataPoint(koeffs, dates, orderCalc);
            //chartKoeff.AddSeriesOrUpdateApprox(dates, Aavg);

            Ravg = ApproxR.CalcDataPoint(resists, dates, orderCalc);
            chartResist.AddSeriesOrUpdateApprox(dates, Ravg);

            ResistFunc = ApproxR.GetStringFunction();
        }
    }
}
