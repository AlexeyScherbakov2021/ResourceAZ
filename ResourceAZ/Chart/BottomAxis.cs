using OxyPlot.Axes;
using ResourceAZ.Models;
using ResourceAZ.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceAZ.Chart
{
    class BottomAxis : DateTimeAxis
    {
        MainWindowViewModel viewModel;


        public BottomAxis(MainWindowViewModel model)
        {
            viewModel = model;
        }

        public override void ZoomAt(double factor, double x)
        {
        }

        public override void Zoom(double x0, double x1)
        {
            if (x0 < DataMinimum)
                x0 = DataMinimum;

            if (x1 > DataMaximum)
                x1 = DataMaximum;

            if (viewModel.MinSelectedValue != DateTime.FromOADate(x0) || viewModel.MaxSelectedValue != DateTime.FromOADate(x1))
            {
                viewModel.MinSelectedValue = DateTime.FromOADate(x0);
                viewModel.MaxSelectedValue = DateTime.FromOADate(x1);

                if (this != viewModel.ModelCurrent.Axes[0])
                {
                    viewModel.ModelCurrent.Axes[0].Zoom(x0, x1);
                    viewModel.ModelCurrent.InvalidatePlot(true);
                }

                if (this != viewModel.ModelNapr.Axes[0])
                {
                    viewModel.ModelNapr.Axes[0].Zoom(x0, x1);
                    viewModel.ModelNapr.InvalidatePlot(true);
                }

                if (this != viewModel.ModelPot.Axes[0])
                {
                    viewModel.ModelPot.Axes[0].Zoom(x0, x1);
                    viewModel.ModelPot.InvalidatePlot(true);
                }

                if (this != viewModel.ModelA.Axes[0])
                {
                    viewModel.ModelA.Axes[0].Zoom(x0, x1);
                    viewModel.ModelA.InvalidatePlot(true);
                }

                if (this != viewModel.ModelR.Axes[0])
                {
                    viewModel.ModelR.Axes[0].Zoom(x0, x1);
                    viewModel.ModelR.InvalidatePlot(true);
                }
            }

            //base.Zoom(x0, x1);

            // выделение строк в datagrid
            viewModel.SelectRangeDataGrid(DateTime.FromOADate(x0), DateTime.FromOADate(x1));

        }


        //protected override string FormatValueOverride(double x)
        //{
        //    return x.ToString();
        //}

    }
}
