using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResourceAZ.Chart;
using ResourceAZ.Models;
using ResourceAZ.ViewModels;

namespace ResourceAZ.Calculation
{
    internal class CalculateResist : CalculateBase
    {
        int LimitYearNapr = 0;
        double[] ApproxR;
        double EndValueCurrent;

        public CalculateResist(MainWindowViewModel model) : base(model)
        {
            ApproxR = model.ApproxR.ApproxDigit;
        }



        public override ObservableCollection<Measure> Calc()
        {

            IEnumerable<Measure> rangeList = model.SetSelectedRange ?
                model.listMeasure.Where(m => m.date >= StartDate && m.date <= EndDate)
                : model.listMeasure;


            //double StartValueR = model.Ravg[0];
            double EndValueR = model.Ravg[indexEndR];

            if (model.AvgCurrent is null)
            {
                // рассчитываем среднее от 20 последних показаний тока
                int Last20Current = rangeList.Count() >= 20 ? rangeList.Count() - 20 : 0;
                EndValueCurrent = rangeList.Skip(Last20Current).Average(a => a.Current);
            }
            else
                EndValueCurrent = model.AvgCurrent.Value;

            TimeSpan dateSub = EndDate.Subtract(StartDate);
            double Years = dateSub.Days / 365.0;

            double y;

            do
            {
                Measure meas = new Measure();
                y = 0;
                for (int i = 0; i < ApproxR.Length; i++)
                    y += ApproxR[i] * Math.Pow(EndDate.ToOADate(), i);
                EndValueR = y;

                if (EndValueR <= 0)
                    break;
                EndDate = EndDate.AddYears(1);
                if(EndDate.Year > 2260)
                    break;

                meas.date = EndDate;
                meas.Resist = EndValueR;

                meas.Current = EndValueCurrent;
                meas.Napr = EndValueCurrent * meas.Resist;
                listCalcMeasure.Add(meas);

                if (LimitYearNapr == 0 && meas.Napr >= model.MaxNaprSKZ)
                {
                    LimitYearNapr = EndDate.Year - 1;
                    int ind = listCalcMeasure.IndexOf(meas);
                    if(ind >= 1)
                        listCalcMeasure[ind - 1].SetColor = true;
                }

            } while (listCalcMeasure[listCalcMeasure.Count - 1].Napr < model.MaxNaprSKZ);

            return listCalcMeasure;
        }

        //----------------------------------------------------------------------------------------------------
        // строка результата
        //----------------------------------------------------------------------------------------------------
        public override List<string> ResultText()
        {
            List<string> resList = new List<string>
            {
                "Тип расчета:  По сопротивлению",
                $"Выходной ток для расчета {EndValueCurrent:F2}",
                $"Предельный режим СКЗ для анодного заземлителя будет достигнут в { LimitYearNapr} году.",
            };

            return resList;
        }

    }
}
