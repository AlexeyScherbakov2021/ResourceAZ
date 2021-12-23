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
    internal class CalculateResist : ICalculateBase
    {
        ObservableCollection<Measure> listCalcMeasure;
        int LimitYearNapr = 0;

        public CalculateResist()
        {
            listCalcMeasure = new ObservableCollection<Measure>();
        }

        public ObservableCollection<Measure> Calc(MainWindowViewModel model)
        {
            double deltaR;
            double StartValueR = model.dpRavg[0].Y;
            double EndValueR = model.dpRavg[model.dpRavg.Count - 1].Y;
            double EndValueNapr = model.listMeasure[model.listMeasure.Count - 1].Napr;

            // рассчитываем среднее от 20 последних показаний тока
            int Last20Current = model.listMeasure.Count >= 20 ? model.listMeasure.Count - 20 : 0;
            double EndValueCurrent = model.listMeasure.Skip(Last20Current).Average(a => a.Current);

            //EndValueCurrent = model.MaxCurrentSKZ;

            // получаем крайние даты
            DateTime EndDate = model.listMeasure[model.listMeasure.Count - 1].date;
            DateTime StartDate = model.listMeasure[0].date;

            TimeSpan dateSub = EndDate.Subtract(StartDate);
            double Years = dateSub.Days / 365.0;

            //deltaR = (StartValueR - EndValueR) / Years;
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
        public List<string> ResultText()
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
