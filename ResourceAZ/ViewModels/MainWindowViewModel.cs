using ResourceAZ.Infrastructure.Commands;
using ResourceAZ.Models;
using ResourceAZ.Repository;
using ResourceAZ.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using OxyPlot;
using OxyPlot.Series;
//using LiveCharts;
//using LiveCharts.Wpf;
using System.Windows.Media;
using OxyPlot.Axes;

namespace ResourceAZ.ViewModels
{
     internal class MainWindowViewModel : ViewModel
    {
        public List<Measure> listMeasure { get; set; }
        private IMeasureData repository = new MeasureDataGen();
        public PlotModel Model { get; }


        #region Команды
        public ICommand CloseApplicationCommand { get; }
        private bool CanCloseApplicationCommand(object p) => true;

        private void OnCloseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }
        #endregion


        public MainWindowViewModel()
        {
            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommand);

            listMeasure = repository.GetAllData();

            Model = new PlotModel();
            List<DataPoint> listDP = ModelToChart(listMeasure);
            LineSeries ls = new LineSeries();
            ls.ItemsSource = listDP;
            Model.Series.Add(ls);
            var XAxis = new DateTimeAxis();
            XAxis.AxislineStyle = LineStyle.Dot;
            XAxis.Title = "Дата";
            XAxis.StringFormat = "dd.MM.yyyy";
            XAxis.MajorGridlineStyle = LineStyle.Dot;
            Model.Axes.Add(XAxis);
            var YAxis = new LinearAxis();
            YAxis.Title = "Выходной ток";

            Model.Axes.Add(YAxis);
        }


        //public PlotModel Model1
        //{
        //    get
        //    {
        //        var model = new PlotModel();
        //        model.Series.Add(new LineSeries { Title = "Series 1", TrackerKey = "Tracker1", ItemsSource = new List<DataPoint> { new DataPoint(0, 0), new DataPoint(10, 20), new DataPoint(20, 18) } });
        //        model.Series.Add(new LineSeries { Title = "Series 2", TrackerKey = "Tracker2", ItemsSource = new List<DataPoint> { new DataPoint(0, 10), new DataPoint(10, 10), new DataPoint(20, 16) } });
        //        model.Series[0].IsVisible = false;
        //        return model;
        //    }
        //}


        private List<DataPoint> ModelToChart(List<Measure> meas)
        {
            List<DataPoint> list = new List<DataPoint>();

            foreach(Measure m in meas)
            {
                list.Add(new DataPoint(m.date.ToOADate(), m.Current));
            }

            return list;
        }

    }
}
