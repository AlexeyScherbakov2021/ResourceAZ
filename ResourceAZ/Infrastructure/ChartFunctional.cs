using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using ResourceAZ.Chart;
using ResourceAZ.Models;
using ResourceAZ.ScottChart;
using ResourceAZ.ViewModels.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ResourceAZ.ViewModels
{

    internal partial class MainWindowViewModel : ViewModel
    {
        //public ObservableCollection<DataPoint> InitChart(PlotModel model, string title)
        //{
        //    ObservableCollection<DataPoint>  dp = new ObservableCollection<DataPoint>();
        //    LineSeries ls = new LineSeries();
        //    ls.ItemsSource = dp;
        //    model.Series.Add(ls);
        //    ls.Color = OxyColors.Blue;
        //    ls.MarkerType = MarkerType.Circle;
        //    ls.MarkerSize = 3;
        //    ls.StrokeThickness = 2;

        //    var XAxis = new BottomAxis(this);
        //    XAxis.AxislineStyle = LineStyle.Dot;
        //    XAxis.StringFormat = "MM.yyyy";
        //    XAxis.MajorGridlineStyle = LineStyle.Automatic;
        //    XAxis.MinorGridlineStyle = LineStyle.Dot;
        //    XAxis.FontSize = 11;
        //    XAxis.IsPanEnabled = false;
        //    //XAxis.IntervalLength = 40;
        //    XAxis.IntervalType = DateTimeIntervalType.Months;
        //    model.Axes.Add(XAxis);

        //    var YAxis = new LinearAxis();
        //    YAxis.MajorGridlineStyle = LineStyle.Automatic;
        //    YAxis.MinorGridlineStyle = LineStyle.Dot;
        //    YAxis.FontSize = 11;
        //    model.Title = title;
        //    YAxis.IsZoomEnabled = false;
        //    YAxis.IsPanEnabled = false;
        //    YAxis.IntervalLength = 20;
        //    YAxis.MinimumMajorStep = 0.1;
        //    model.Axes.Add(YAxis);

        //    model.TitleFontSize = 13;
        //    model.PlotMargins = new OxyThickness(20, 0, 5, 20);

        //    return dp;
        //}


        //--------------------------------------------------------------------------------------------
        // перенос данных а график
        //--------------------------------------------------------------------------------------------
        private void ModelToChart(ObservableCollection<Measure> meas)
        {
            dates = new double[meas.Count];
            currents = new double[meas.Count];
            naprs = new double[meas.Count];
            sumpots = new double[meas.Count];
            koeffs = new double[meas.Count];
            resists = new double[meas.Count];

            int i = 0;
            // заполнение точками списков для графиков
            foreach (Measure m in meas)
            {
                dates[i] = m.date.ToOADate();
                currents[i] = m.Current;
                naprs[i] = m.Napr;
                sumpots[i] = m.SummPot;
                koeffs[i] = m.Koeff;
                resists[i] = m.Resist;
                i++;
            }

            // обновление точек графиков на экране
            chartCurrent.AddSeriesOrUpdate(dates, currents, "Выходной ток");
            chartNapr.AddSeriesOrUpdate(dates, naprs, "Напряжение");
            chartPot.AddSeriesOrUpdate(dates, sumpots, "Потенциал");
            chartKoeff.AddSeriesOrUpdate(dates, koeffs, "Коэффициенты");
            chartResist.AddSeriesOrUpdate(dates, resists, "Сопротивление");

        }

        //--------------------------------------------------------------------------------------------
        // расчет аппроксимации линии
        // 
        //--------------------------------------------------------------------------------------------
        //private double[] CalcApproxLine(scottChart chart, double[] dp, double[] datesRange, KindLineApprox kind, double EndX = -1, double Step = 10)
        //{
        //    double[] dpAvg = CalcDataPoint(dp, datesRange, kind, EndX, Step);

        //    chart.AddSeriesOrUpdateApprox(datesRange, dpAvg);
        //    return dpAvg;
        //}

    }
}