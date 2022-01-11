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
    internal class CalculatePotencial : CalculateBase
    {
        int LimitYearCurr = 0;
        int LimitYearNapr = 0;

        public CalculatePotencial(MainWindowViewModel model) : base(model)
        {
        }

        //public ObservableCollection<Measure> Calc(ObservableCollection<Measure> listMeasure, double MinSummPot, double maxCur, double maxNapr)
        public override ObservableCollection<Measure> Calc( )
        {
            double deltaA;
            double deltaR;
            double StartValueA = model.dpAavg[0].Y;
            double EndValueA = model.dpAavg[indexEnd].Y;
            double StartValueR = model.dpRavg[0].Y;
            double EndValueR = model.dpRavg[indexEnd].Y;

            //double StartValueA = listMeasure[0].Koeff;
            //double EndValueA = listMeasure[listMeasure.Count - 1].Koeff;
            //double StartValueR = listMeasure[0].Resist;
            //double EndValueR = listMeasure[listMeasure.Count - 1].Resist;

            //DateTime EndDate = model.listMeasure[model.listMeasure.Count - 1].date;
            //DateTime StartDate = model.listMeasure[0].date;

            TimeSpan dateSub = EndDate.Subtract(StartDate);
            double Years = dateSub.Days / 365.0;

            deltaA = (StartValueA - EndValueA) / Years;
            deltaR = (StartValueR - EndValueR) / Years;

            do
            {
                Measure meas = new Measure();
                EndValueA -= deltaA;
                EndValueR -= deltaR;
                if (EndValueR <= 0.05)
                    EndValueR = 0.05;
                if (EndValueA >= -0.05)
                    EndValueA = -0.05;
                EndDate = EndDate.AddYears(1);
                meas.date = EndDate;
                meas.Koeff = EndValueA;
                meas.Resist = EndValueR;
                meas.Current = model.MinPotCalc / meas.Koeff;
                meas.Napr = meas.Current * meas.Resist;
                listCalcMeasure.Add(meas);

                if (LimitYearCurr == 0 && meas.Current >= model.MaxCurrentSKZ)
                    LimitYearCurr = EndDate.Year - 1;

                if (LimitYearNapr == 0 && meas.Napr >= model.MaxNaprSKZ)
                {
                    LimitYearNapr = EndDate.Year - 1;
                    int ind = listCalcMeasure.IndexOf(meas);
                    if (ind > 0)
                        listCalcMeasure[ind - 1].SetColor = true;
                }

            } while ((listCalcMeasure[listCalcMeasure.Count - 1].Current <= model.MaxCurrentSKZ ||
                            listCalcMeasure[listCalcMeasure.Count - 1].Napr <= model.MaxNaprSKZ) && EndDate.Year < 2260);

            return listCalcMeasure;

        }


        //----------------------------------------------------------------------------------------------------
        // строка результата
        //----------------------------------------------------------------------------------------------------
        public override List<string> ResultText()
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
