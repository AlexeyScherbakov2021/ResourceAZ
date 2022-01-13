using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceAZ.ScottChart
{
    class scottChart
    {
        private WpfPlot _chart;
        ScatterPlot mainPlot;
        ScatterPlot apprPlot;

        public scottChart(WpfPlot chart)
        {
            _chart = chart;
            //_chart.Plot.XAxis.Label("Время");
            _chart.Plot.XAxis.DateTimeFormat(true);
            _chart.Plot.XAxis.Grid(true);
            _chart.Plot.XAxis.Layout(0);
            _chart.Plot.XAxis.ManualTickSpacing(1);
            //_chart.Plot.XAxis.`
            //_chart.Plot.XAxis.TickLabelStyle(null, null, 10, false, 30);
            //_chart.Plot.YAxis.Layout(0.1f);
            //_chart.Plot.Layout(0,0,0,0, -8);
            _chart.Plot.Grid(true, Color.LightGray);
            
        }

        public void AddSeriesOrUpdate(double[] X, double[] Y, string Name = "")
        {
            if (mainPlot is null)
            {
                mainPlot = _chart.Plot.AddScatter(X, Y);
                //mainPlot.Label = Name;
                _chart.Plot.Title(Name, false);
                _chart.Plot.XAxis2.Label(Name);
                mainPlot.LineWidth = 2;
                mainPlot.MarkerShape = MarkerShape.filledCircle;
                mainPlot.MarkerSize = 5;
                mainPlot.Color = Color.Blue;

            }
            else
                mainPlot.Update(X, Y);

            _chart.Plot.AxisAuto();
            _chart.Refresh();
        }


        public void AddSeriesOrUpdateApprox(double[] X, double[] Y)
        {
            if (apprPlot is null)
            {
                apprPlot = _chart.Plot.AddScatter(X, Y);
                apprPlot.Color = Color.Red;
                apprPlot.LineWidth = 3;
                apprPlot.MarkerShape = MarkerShape.none;

            }
            else
                apprPlot.Update(X, Y);

            _chart.Refresh();
        }
    }
}
