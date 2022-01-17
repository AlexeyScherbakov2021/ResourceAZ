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
        //protected int indexStart;
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

            if(model.SetSelectedRange)
            {
                rangeList = model.listMeasure.Where(m => m.date >= MinDateValue && m.date <= MaxDateValue);
             }
            else
            {
                rangeList = model.listMeasure;
            }

            indexEnd = model.ApproxR.endIndex;

            StartDate = DateTime.FromOADate(model.dates[Array.IndexOf(model.dates, 
                model.dates.FirstOrDefault(n => n >= (double.IsNaN(model.X1) ? 0 : model.X1)))]);
            EndDate = DateTime.FromOADate(model.dates[Array.IndexOf(model.dates, 
                model.dates.LastOrDefault(n => n <= (double.IsNaN(model.X2) ? double.MaxValue : model.X2)))]);

            listCalcMeasure = new ObservableCollection<Measure>();
        }

        public abstract ObservableCollection<Measure> Calc();

        public abstract List<string> ResultText();


    }
}
