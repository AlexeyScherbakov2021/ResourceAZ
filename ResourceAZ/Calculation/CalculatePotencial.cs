using ResourceAZ.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceAZ.Calculation
{
    internal class CalculatePotencial : ICalculateBase
    {
        ObservableCollection<Measure> listCalcMeasure;
        int LimitYearCurr = 0;
        int LimitYearNapr = 0;

        public CalculatePotencial()
        {
            listCalcMeasure = new ObservableCollection<Measure>();
        }

        public ObservableCollection<Measure> Calc(ObservableCollection<Measure> listMeasure, double MinSummPot, double maxCur, double maxNapr)
        {
            double deltaA;
            double deltaR;
            double StartValueA = listMeasure[0].Koeff;
            double EndValueA = listMeasure[listMeasure.Count - 1].Koeff;
            double StartValueR = listMeasure[0].Resist;
            double EndValueR = listMeasure[listMeasure.Count - 1].Resist;

            DateTime EndDate = listMeasure[listMeasure.Count - 1].date;
            DateTime StartDate = listMeasure[0].date;

            TimeSpan dateSub = EndDate.Subtract(StartDate);
            double Years = dateSub.Days / 365.0;

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
                listCalcMeasure.Add(meas);

                if (LimitYearCurr == 0 && meas.Current >= maxCur)
                    LimitYearCurr = EndDate.Year - 1;

                if (LimitYearNapr == 0 && meas.Napr >= maxNapr)
                    LimitYearNapr = EndDate.Year - 1;

            } while ((listCalcMeasure[listCalcMeasure.Count - 1].Current <= maxCur ||
                            listCalcMeasure[listCalcMeasure.Count - 1].Napr <= maxNapr) && EndDate.Year < 2260);

            return listCalcMeasure;

        }


        //----------------------------------------------------------------------------------------------------
        // строка результата
        //----------------------------------------------------------------------------------------------------
        public List<string> ResultText()
        {
            List<string> resList = new List<string>
            {
                "Тип расчета:  По потенциалу",
                $"Предельный режим СКЗ для анодного заземлителя будет достигнут в { Math.Min(LimitYearCurr, LimitYearNapr)} году.",
                $"По току: в {LimitYearCurr} году",
                $"По напряжению: в {LimitYearNapr} году"
            };

            return resList;
        }
    }
}
