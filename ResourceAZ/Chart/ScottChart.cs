using Microsoft.Win32;
using ResourceAZ.ViewModels;
using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ResourceAZ.ScottChart
{
    class scottChart
    {
        private WpfPlot _chart;
        ScatterPlot mainPlot;
        ScatterPlot apprPlot;
        HSpan span;
        private readonly MainWindowViewModel _vm;

        public scottChart(WpfPlot chart, MainWindowViewModel vm)
        {
            _chart = chart;
            _vm = vm;
            _vm.listPlot.Add(this);
            _chart.RightClicked -= _chart.DefaultRightClickEvent;
            _chart.RightClicked += DefaultRightClickEvent;

            //_chart.Plot.XAxis.Label("Время");
            _chart.Plot.XAxis.DateTimeFormat(true);
            _chart.Plot.XAxis.Grid(true);
            //_chart.Plot.XAxis.Layout(0);
            //_chart.Plot.XAxis.ManualTickSpacing(1);
            //_chart.Plot.XAxis.`
            //_chart.Plot.XAxis.TickLabelStyle(null, null, 10, false, 30);
            //_chart.Plot.YAxis.Layout(0.1f);
            _chart.Plot.Layout(0,0,0,0, -3);
            _chart.Plot.Grid(true, Color.LightGray);

            span = _chart.Plot.AddHorizontalSpan(0, 0, Color.FromArgb(140, 0, 255, 255));
            span.Dragged += Span_Dragged;
            span.IsVisible = false;

            double[] x = new double[1] { 0 };
            AddSeriesOrUpdate(x, x);

        }


        // Контекстное меню
        public void DefaultRightClickEvent(object sender, EventArgs e)
        {
            var cm = new ContextMenu();

            MenuItem SaveImageMenuItem = new MenuItem() { Header = "Сохранить изображение" };
            SaveImageMenuItem.Click += RightClickMenu_SaveImage_Click;
            cm.Items.Add(SaveImageMenuItem);

            //MenuItem CopyImageMenuItem = new MenuItem() { Header = "Скопировать изображение" };
            //CopyImageMenuItem.Click += RightClickMenu_Copy_Click;
            //cm.Items.Add(CopyImageMenuItem);

            MenuItem AutoAxisMenuItem = new MenuItem() { Header = "Вместить данные в окно" };
            AutoAxisMenuItem.Click += RightClickMenu_AutoAxis_Click;
            cm.Items.Add(AutoAxisMenuItem);

            MenuItem OpenInNewWindowMenuItem = new MenuItem() { Header = "Открыть в новом окне" };
            OpenInNewWindowMenuItem.Click += RightClickMenu_OpenInNewWindow_Click;
            cm.Items.Add(OpenInNewWindowMenuItem);

            cm.IsOpen = true;
        }

        //private void RightClickMenu_Copy_Click(object sender, EventArgs e) => 
        //    System.Windows.Clipboard.SetImage(BmpImageFromBmp(Backend.GetLatestBitmap()));
        private void RightClickMenu_OpenInNewWindow_Click(object sender, EventArgs e) => new WpfPlotViewer(_chart.Plot).Show();
        private void RightClickMenu_AutoAxis_Click(object sender, EventArgs e) { _chart.Plot.AxisAuto(); _chart.Refresh(); }
        private void RightClickMenu_SaveImage_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                FileName = "ScottPlot.png",
                Filter = "PNG Files (*.png)|*.png;*.png" +
                         "|JPG Files (*.jpg, *.jpeg)|*.jpg;*.jpeg" +
                         "|BMP Files (*.bmp)|*.bmp;*.bmp" +
                         "|All files (*.*)|*.*"
            };

            if (sfd.ShowDialog() is true)
                _chart.Plot.SaveFig(sfd.FileName);
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

            _chart.Plot.AxisAuto();
            _chart.Refresh();
        }


        public void SetSelectedRange(bool enable, double X1 = 0,  double X2 = 40000 )
        {
            foreach (scottChart plot in _vm.listPlot)
            {
                plot.span.X1 = X1;
                plot.span.X2 = X2;
                plot.span.IsVisible = enable;
                plot.span.DragEnabled = enable;
                plot._chart.Refresh();
            }

        }

        private void Span_Dragged(object sender, EventArgs e)
        {
            foreach(scottChart plot in _vm.listPlot)
            {
                if (plot == this) continue;

                plot.span.X1 = span.X1;
                plot.span.X2 = span.X2;
                plot._chart.Refresh();
            }

            _vm.X1 = Math.Min(span.X1, span.X2);
            _vm.X2 = Math.Max(span.X1, span.X2);

            _vm.SelectRangeDataGrid(DateTime.FromOADate(_vm.X1), DateTime.FromOADate(_vm.X2));

        }

    }
}