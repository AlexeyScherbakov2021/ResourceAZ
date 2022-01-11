using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResourceAZ.Models;
using ResourceAZ.ViewModels;

namespace ResourceAZ.Calculation
{
    internal class CalculateResist : CalculateBase
    {
        int LimitYearNapr = 0;

        public CalculateResist(MainWindowViewModel model) : base(model)
        {
        }

        public override ObservableCollection<Measure> Calc(/*MainWindowViewModel model*/)
        {

            //IEnumerable<Measure> rangeList = model.RangeForCalc ?
            //    model.listMeasure.Where(m => m.date >= model.MinSelectedValue && m.date <= model.MaxSelectedValue)
            //    : model.listMeasure; 

            //int indexEnd = model.RangeForCalc ?
            //    model.dpRavg.IndexOf(model.dpRavg.Where(a => a.X <= model.MaxSelectedValue.ToOADate()).Last())
            //    : model.dpRavg.Count - 1;


            double StartValueR = model.dpRavg[0].Y;
            double EndValueR = model.dpRavg[indexEnd].Y;
            //double EndValueNapr = model.listMeasure[model.EndRange].Napr;

            // рассчитываем среднее от 20 последних показаний тока
            int Last20Current = rangeList.Count() >= 20 ? rangeList.Count() - 20 : 0;
            double EndValueCurrent = rangeList.Skip(Last20Current).Average(a => a.Current);

            // получаем крайние даты
            //DateTime StartDate = DateTime.FromOADate(model.dpRavg[0].X);
            //DateTime EndDate = DateTime.FromOADate(model.dpRavg[indexEnd].X);

            TimeSpan dateSub = EndDate.Subtract(StartDate);
            double Years = dateSub.Days / 365.0;

            double y;

            do
            {
                Measure meas = new Measure();
                y = 0;
                for (int i = 0; i < model.ApproxR.Length; i++)
                    y += model.ApproxR[i] * Math.Pow(EndDate.ToOADate(), i);
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
                $"Предельный режим СКЗ для анодного заземлителя будет достигнут в { LimitYearNapr} году.",
            };

            return resList;
        }

    }
}
