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
            MinDateValue = model.MinSelectedValue;
            MaxDateValue = model.MaxSelectedValue;

            if(model.RangeForCalc)
            {
                rangeList = model.listMeasure.Where(m => m.date >= MinDateValue && m.date <= MaxDateValue);
                //indexStart = model.dpRavg.IndexOf(model.dpRavg.Where(a => a.X <= model.MinSelectedValue.ToOADate()).First());
                //indexEnd = model.dpRavg.IndexOf(model.dpRavg.Where(a => a.X <= MaxDateValue.ToOADate()).Last());
            }
            else
            {
                //indexStart = 0;
                rangeList = model.listMeasure;
                //indexEnd = model.dpRavg.Count - 1;
            }

            //StartDate = DateTime.FromOADate(model.dpRavg[0].X);
            //EndDate = DateTime.FromOADate(model.dpRavg[indexEnd].X);


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
