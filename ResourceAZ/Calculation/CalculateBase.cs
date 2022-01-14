using ResourceAZ.Chart;
using ResourceAZ.Models;
using ResourceAZ.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceAZ.Calculation
{
    abstract class CalculateBase
    {
        protected ObservableCollection<Measure> listCalcMeasure;
        protected IEnumerable<Measure> rangeList;
        protected DateTime MinDateValue;
        protected DateTime MaxDateValue;
        protected MainWindowViewModel model;
        protected int indexStart;
        protected int indexEnd;
        protected DateTime StartDate;
        protected DateTime EndDate;

        public CalculateBase(MainWindowViewModel model)
        {
            this.model = model;
            MinDateValue = double.IsNaN(model.X1) ? DateTime.FromOADate(model.dates[0]) : DateTime.FromOADate(model.X1);
            MaxDateValue = double.IsNaN(model.X2) 
                ? DateTime.FromOADate(model.dates[model.dates.Length - 1]) 
                : DateTime.FromOADate(model.X2);

            if(model.RangeForCalc)
            {
                rangeList = model.listMeasure.Where(m => m.date >= MinDateValue && m.date <= MaxDateValue);
                //indexStart = model.dpRavg.IndexOf(model.dpRavg.Where(a => a.X <= model.MinSelectedValue.ToOADate()).First());
                indexStart = Array.IndexOf(model.dates, model.dates.FirstOrDefault(n => n >= model.X1));
                //indexEnd = model.dpRavg.IndexOf(model.dpRavg.Where(a => a.X <= MaxDateValue.ToOADate()).Last());
                indexEnd = Array.IndexOf(model.dates, model.dates.LastOrDefault(n => n <= model.X2));
             }
            else
            {
                //indexStart = 0;
                rangeList = model.listMeasure;
                indexEnd = model.Ravg.Length - 1;
            }

            StartDate = DateTime.FromOADate(model.dates[indexStart]);
            EndDate = DateTime.FromOADate(model.dates[indexEnd]);

            listCalcMeasure = new ObservableCollection<Measure>();
            //listMeasure = model.RangeForCalc 
            //    ? model.listMeasure.Where(m => m.date >= MinDateValue && m.date <= MaxDateValue)
            //    : model.listMeasure;
        }

        //ObservableCollection<Measure> Calc(ObservableCollection<Measure> listMeasure, double MinSummPot, double maxCur, double maxNapr);
        public abstract ObservableCollection<Measure> Calc(/*MainWindowViewModel model*/);

        public abstract List<string> ResultText();


    }
}
