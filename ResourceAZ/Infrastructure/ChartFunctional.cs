using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using ResourceAZ.Chart;
using ResourceAZ.Models;
using ResourceAZ.ViewModels.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ResourceAZ.ViewModels
{

    internal partial class MainWindowViewModel : ViewModel
    {
        public List<DataPoint> InitChart(PlotModel model, string title)
        {
            List<DataPoint>  dp = new List<DataPoint>();
            LineSeries ls = new LineSeries();
            ls.ItemsSource = dp;
            model.Series.Add(ls);
            ls.Color = OxyColors.Blue;
            ls.MarkerType = MarkerType.Circle;
            ls.MarkerSize = 3;
            ls.StrokeThickness = 2;

            var XAxis = new BottomAxis(this);
            XAxis.AxislineStyle = LineStyle.Dot;
            XAxis.StringFormat = "MM.yyyy";
            XAxis.MajorGridlineStyle = LineStyle.Automatic;
            XAxis.MinorGridlineStyle = LineStyle.Dot;
            XAxis.FontSize = 11;
            XAxis.IsPanEnabled = false;
            //XAxis.IntervalLength = 40;
            XAxis.IntervalType = DateTimeIntervalType.Months;
            model.Axes.Add(XAxis);

            var YAxis = new LinearAxis();
            YAxis.MajorGridlineStyle = LineStyle.Automatic;
            YAxis.MinorGridlineStyle = LineStyle.Dot;
            YAxis.FontSize = 11;
            model.Title = title;
            YAxis.IsZoomEnabled = false;
            YAxis.IsPanEnabled = false;
            YAxis.IntervalLength = 20;
            YAxis.MinimumMajorStep = 0.1;
            model.Axes.Add(YAxis);

            model.TitleFontSize = 13;
            model.PlotMargins = new OxyThickness(20, 0, 5, 20);

            return dp;
        }


        //--------------------------------------------------------------------------------------------
        // перенос данных а график
        //--------------------------------------------------------------------------------------------
        private void ModelToChart(ObservableCollection<Measure> meas)
        {
            dpCurrent.Clear();
            dpNapr.Clear();
            dpPot.Clear();
            dpA.Clear();
            dpR.Clear();

            // заполнение точками срисков для графиков
            foreach (Measure m in meas)
            {
                dpCurrent.Add(new DataPoint(m.date.ToOADate(), m.Current));
                dpNapr.Add(new DataPoint(m.date.ToOADate(), m.Napr));
                dpPot.Add(new DataPoint(m.date.ToOADate(), m.SummPot));
                dpA.Add(new DataPoint(m.date.ToOADate(), m.Koeff));
                dpR.Add(new DataPoint(m.date.ToOADate(), m.Resist));

            }

            // обновление точек графиков на экране
            ModelCurrent.InvalidatePlot(true);
            ModelNapr.InvalidatePlot(true);
            ModelPot.InvalidatePlot(true);
            ModelA.InvalidatePlot(true);
            ModelR.InvalidatePlot(true);
        }

        //--------------------------------------------------------------------------------------------
        // расчет аппроксимации линии
        //--------------------------------------------------------------------------------------------
        //private List<DataPoint> CalcApproxLine(PlotModel model, List<DataPoint> dp, ref double[] Approx)
        private List<DataPoint> CalcApproxLine(PlotModel model, List<DataPoint> dp, KindLineApprox kind)
        {
            List<DataPoint> dpAvg = CalcDataPoint(dp, kind);
            if (model.Series.Count == 1)
            {
                LineSeries ls = new LineSeries();
                ls.Color = OxyColor.FromRgb(255, 0, 0);
                //ls.MarkerType = MarkerType.Circle;
                ls.StrokeThickness = 3;
                model.Series.Add(ls);

            }
            model.Series[1].IsVisible = true;
            (model.Series[1] as LineSeries).ItemsSource = dpAvg;
            model.InvalidatePlot(true);

            return dpAvg;
        }

    }
}