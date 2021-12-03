using ResourceAZ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceAZ.Calculation
{
    internal class CalculatePotencial : CalculateBase
    {

        public CalculatePotencial(Measure m) : base(m)
        {

        }

        public override void Calc()
        {
            double StartValueA = listMeasure[0].Koeff;
            double EndValueA = listMeasure[listMeasure.Count - 1].Koeff;
            double StartValueR = listMeasure[0].Resist;
            double EndValueR = listMeasure[listMeasure.Count - 1].Resist;

            DateTime EndDate = listMeasure[listMeasure.Count - 1].date;
            DateTime StartDate = listMeasure[0].date;

            TimeSpan dateSub = EndDate.Subtract(StartDate);
            double Years = dateSub.Days / 365;

            deltaA = (StartValueA - EndValueA) / Years;
            deltaR = (StartValueR - EndValueR) / Years;

            do
            {
                Measure meas = new Measure();
                EndValueA -= deltaA;
                EndValueR -= deltaR;
                if (EndValueA >= 0 || EndValueR <= 0)
                    break;
                EndDate = EndDate.AddYears(1);
                meas.date = EndDate;
                meas.Koeff = EndValueA;
                meas.Resist = EndValueR;
                meas.Current = MinSummPot / meas.Koeff;
                meas.Napr = meas.Current * meas.Resist;
                listMeasure.Add(meas);

                if (LimitYearCurr == 0 && meas.Current >= model.MaxCurrentSKZ)
                    LimitYearCurr = EndDate.Year - 1;

                if (LimitYearNapr == 0 && meas.Napr >= model.MaxNaprSKZ)
                    LimitYearNapr = EndDate.Year - 1;

            } while (listMeasure[listMeasure.Count - 1].Current <= model.MaxCurrentSKZ ||
                            listMeasure[listMeasure.Count - 1].Napr <= model.MaxNaprSKZ);

            StartDateTime = listMeasure[listMeasure.Count - 1].date.AddYears(1);
            EndDateTime = EndDate;


        }
    }
}
