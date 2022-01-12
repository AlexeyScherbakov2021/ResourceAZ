using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using ResourceAZ.Chart;
using ResourceAZ.Models;
using ResourceAZ.ViewModels.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ResourceAZ.ViewModels
{

    internal partial class MainWindowViewModel : ViewModel
    {
        public ObservableCollection<DataPoint> InitChart(PlotModel model, string title)
        {
            ObservableCollection<DataPoint>  dp = new ObservableCollection<DataPoint>();
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
            //dpCurrent.Clear();
            //dpNapr.Clear();
            //dpPot.Clear();
            //dpA.Clear();
            //dpR.Clear();

            // заполнение точками списков для графиков
            foreach (Measure m in meas)
            {
                //dpCurrent.Add(new DataPoint(m.date.ToOADate(), m.Current));
                //dpNapr.Add(new DataPoint(m.date.ToOADate(), m.Napr));
                //dpPot.Add(new DataPoint(m.date.ToOADate(), m.SummPot));
                //dpA.Add(new DataPoint(m.date.ToOADate(), m.Koeff));
                //dpR.Add(new DataPoint(m.date.ToOADate(), m.Resist));

            }

            // обновление точек графиков на экране
            //ModelCurrent.InvalidatePlot(true);
            //ModelNapr.InvalidatePlot(true);
            //ModelPot.InvalidatePlot(true);
            //ModelA.InvalidatePlot(true);
            //ModelR.InvalidatePlot(true);

            //if (ModelR.DefaultYAxis != null && dpR.Count > 0 && dpA.Count > 0 )
            //{
            //    ModelR.DefaultYAxis.Maximum = dpR.Max(m => m.Y);
            //    ModelR.DefaultYAxis.Minimum = dpR.Min(m => m.Y);
            //    ModelA.DefaultYAxis.Maximum = dpA.Max(m => m.Y);
            //    ModelA.DefaultYAxis.Minimum = dpA.Min(m => m.Y);
            //}

        }

        //--------------------------------------------------------------------------------------------
        // расчет аппроксимации линии
        // 
        //--------------------------------------------------------------------------------------------
        private ObservableCollection<DataPoint> CalcApproxLine(PlotModel model, ObservableCollection<DataPoint> dp, KindLineApprox kind, double EndX = -1, double Step = 10)
        {
            ObservableCollection<DataPoint> dpAvg = CalcDataPoint(dp, kind, EndX, Step);
            if (model.Series.Count == 1)
            {
                LineSeries ls = new LineSeries();
                ls.Color = OxyColor.FromRgb(255, 0, 0);
                //ls.MarkerType = MarkerType.Circle;
                ls.StrokeThickness = 3;
                model.Series.Add(ls);

            }
            (model.Series[1] as LineSeries).ItemsSource = dpAvg;
            model.InvalidatePlot(true);

            return dpAvg;
        }

    }
}