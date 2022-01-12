using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
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
        }

        public void AddSeriesOrUpdate(double[] X, double[] Y, string Name = "")
        {
            if (mainPlot is null)
            {
                mainPlot = _chart.Plot.AddScatter(X, Y);
                mainPlot.Label = Name;
            }
            else
                mainPlot.Update(X, Y);

            _chart.Refresh();
        }


        public void AddSeriesOrUpdateApprox(double[] X, double[] Y)
        {
            if (apprPlot is null)
                apprPlot = _chart.Plot.AddScatter(X, Y);
            else
                apprPlot.Update(X, Y);

            _chart.Refresh();
        }
    }
}
