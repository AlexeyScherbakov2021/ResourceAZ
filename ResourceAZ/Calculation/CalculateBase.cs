using ResourceAZ.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceAZ.Calculation
{
    internal abstract class CalculateBase
    {
        protected ObservableCollection<Measure> listMeasure;


        public CalculateBase(ObservableCollection<Measure> m)
        {
            listMeasure = m;
        }

        public abstract void Calc();

    }
}
