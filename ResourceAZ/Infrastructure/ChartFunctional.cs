﻿using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using ResourceAZ.Models;
using ResourceAZ.ViewModels.Base;
using System.Collections.ObjectModel;

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
            ls.Color = OxyColor.FromRgb(0, 0, 255);
            ls.MarkerType = MarkerType.Circle;
            ls.StrokeThickness = 3;
            var XAxis = new DateTimeAxis();
            XAxis.AxislineStyle = LineStyle.Dot;
            XAxis.StringFormat = "dd.MM.yyyy";
            XAxis.MajorGridlineStyle = LineStyle.Automatic;
            XAxis.MinorGridlineStyle = LineStyle.Dot;
            XAxis.FontSize = 11;
            model.Axes.Add(XAxis);
            var YAxis = new LinearAxis();
            YAxis.MajorGridlineStyle = LineStyle.Automatic;
            YAxis.MinorGridlineStyle = LineStyle.Dot;
            YAxis.FontSize = 11;
            model.Title = title;
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
        private void CalcApproxLine(PlotModel model, ObservableCollection<DataPoint> dp)
        {
            ObservableCollection<DataPoint> dpAvg = CalcDataPoint(dp);
            if (model.Series.Count == 1)
            {
                LineSeries ls = new LineSeries();
                ls.Color = OxyColor.FromRgb(255, 0, 0);
                ls.MarkerType = MarkerType.Circle;
                ls.StrokeThickness = 3;
                model.Series.Add(ls);

            }
            model.Series[1].IsVisible = true;
            (model.Series[1] as LineSeries).ItemsSource = dpAvg;
            model.InvalidatePlot(true);
        }

    }
}