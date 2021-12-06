using ResourceAZ.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceAZ.Calculation
{
    interface ICalculateBase
    {

        ObservableCollection<Measure> Calc(ObservableCollection<Measure> listMeasure, double MinSummPot, double maxCur, double maxNapr);

    }
}
