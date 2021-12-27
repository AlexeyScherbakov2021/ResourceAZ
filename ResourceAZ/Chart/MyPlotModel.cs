using OxyPlot;
using ResourceAZ.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceAZ.Chart
{
    class MyPlotModel : PlotModel
    {
        //double selMin = 44002;
        //double selMax = 44029;
        MainWindowViewModel viewModel;

        public MyPlotModel(MainWindowViewModel model )
        {
            viewModel = model;
        }

        protected override void RenderOverride(IRenderContext rc, OxyRect rect)
        {
            base.RenderOverride(rc, rect);

            double minX = (viewModel.MinSelectedValue.ToOADate() - DefaultXAxis.Offset) * DefaultXAxis.Scale;
            double maxX = (viewModel.MaxSelectedValue.ToOADate() - DefaultXAxis.Offset) * DefaultXAxis.Scale;

            OxyColor brush = viewModel.RangeForCalc ? OxyColor.FromArgb(70, 0, 255, 255) :
                OxyColor.FromArgb(70, 255, 255, 0);


            rc.DrawRectangle(new OxyRect(minX, PlotArea.Top, maxX - minX, 
                PlotArea.Height), brush, OxyColors.Gray, 1, EdgeRenderingMode.Automatic);
        }

        protected override void OnMouseDown(object sender, OxyMouseDownEventArgs e)
        {
            base.OnMouseDown(sender, e);
        }
    }
}
