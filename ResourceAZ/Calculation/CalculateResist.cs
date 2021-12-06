using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResourceAZ.Models;

namespace ResourceAZ.Calculation
{
    internal class CalculateResist : ICalculateBase
    {
        ObservableCollection<Measure> listCalcMeasure;

        public CalculateResist()
        {
            listCalcMeasure = new ObservableCollection<Measure>();
        }


        public ObservableCollection<Measure> Calc(ObservableCollection<Measure> listMeasure, double MinSummPot, double maxCur, double maxNapr)
        {
            double deltaR;
            int LimitYearCurr = 0;
            int LimitYearNapr = 0;
            double StartValueR = listMeasure[0].Resist;
            double EndValueR = listMeasure[listMeasure.Count - 1].Resist;
            double EndValueCurrent = listMeasure[listMeasure.Count - 1].Current;
            double EndValueNapr = listMeasure[listMeasure.Count - 1].Napr;


            DateTime EndDate = listMeasure[listMeasure.Count - 1].date;
            DateTime StartDate = listMeasure[0].date;

            TimeSpan dateSub = EndDate.Subtract(StartDate);
            double Years = dateSub.Days / 365;

            deltaR = (StartValueR - EndValueR) / Years;

            do
            {
                Measure meas = new Measure();
                EndValueR -= deltaR;
                if (EndValueR <= 0)
                    break;
                EndDate = EndDate.AddYears(1);
                meas.date = EndDate;
                meas.Resist = EndValueR;

                meas.Current = EndValueNapr / meas.Resist;
                meas.Napr = EndValueCurrent * meas.Resist;
                listCalcMeasure.Add(meas);

                if (LimitYearCurr == 0 && meas.Current >= maxCur)
                    LimitYearCurr = EndDate.Year - 1;

                if (LimitYearNapr == 0 && meas.Napr <= 0)
                    LimitYearNapr = EndDate.Year - 1;

            } while (listCalcMeasure[listCalcMeasure.Count - 1].Current <= maxCur /*||
                            listCalcMeasure[listCalcMeasure.Count - 1].Napr > 0*/);

            return listCalcMeasure;
        }
    }
}
