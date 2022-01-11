using OxyPlot;
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
        public PlotModel[] plotModels;

        public BottomAxis(MainWindowViewModel model)
        {
            viewModel = model;
            plotModels = new PlotModel[]
            {
                viewModel.ModelCurrent,
                viewModel.ModelNapr,
                viewModel.ModelPot,
                viewModel.ModelA,
                viewModel.ModelR
            };

        }

        public override void ZoomAt(double factor, double x)
        {
        }

        public override void Zoom(double x0, double x1)
        {
            // ограничиваем выделенный диапазон граничными значениями
            if (x0 < DataMinimum)
                x0 = DataMinimum;

            if (x1 > DataMaximum)
                x1 = DataMaximum;

            // если диапазон изменился
            if (viewModel.MinSelectedValue != DateTime.FromOADate(x0) || viewModel.MaxSelectedValue != DateTime.FromOADate(x1))
            {
                // принимаем новые значения диапазона
                viewModel.MinSelectedValue = DateTime.FromOADate(x0);
                viewModel.MaxSelectedValue = DateTime.FromOADate(x1);

                // вызываем идентичное событие для остальных графиков
                foreach (PlotModel p in plotModels)
                {
                    if (this != p.Axes[0])
                    {
                        p.Axes[0].Zoom(x0, x1);
                        p.InvalidatePlot(true);
                    }
                }

                // выделение строк в datagrid
                viewModel.SelectRangeDataGrid(DateTime.FromOADate(x0), DateTime.FromOADate(x1));
            }
            //base.Zoom(x0, x1);
        }

        //protected override string FormatValueOverride(double x)
        //{
        //    return x.ToString();
        //}

    }
}
