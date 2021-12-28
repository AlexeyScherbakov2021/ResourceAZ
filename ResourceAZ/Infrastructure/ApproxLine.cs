using OxyPlot;
using ResourceAZ.ViewModels.Base;
using System.Collections.ObjectModel;
using MathNet.Numerics.LinearRegression;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
using System;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Collections.Generic;

namespace ResourceAZ.ViewModels
{
    internal partial class MainWindowViewModel : ViewModel
    {

        private List<DataPoint> CalcDataPoint(List<DataPoint> dp, KindLineApprox kind, double EndX = -1, double Step = 10)
        {
            if (dp.Count <= 1)
                return null;

            List<DataPoint> dpApprox = new List<DataPoint>();
            double[] Approx = null;
            string Function = "";


            double X = 0;
            double X2 = 0;
            double Y = 0;
            double XY = 0;
            double N = dp.Count;
            double[] aX = new double[dp.Count];
            double[] aY = new double[dp.Count];

            for(int i = 0; i < dp.Count; i++)
            {
                aX[i] = dp[i].X;
                aY[i] = dp[i].Y;
            }

            for (int order = orderCalc; order > 0; order--)
            {

                try
                {
                    //var AB = SimpleRegression.Fit(aX, aY);

                    //double A = AB.Item2;
                    //double B = AB.Item1;

                    //foreach (DataPoint d in dp)
                    //{
                    //    DataPoint dPoint = new DataPoint(d.X, d.X * A + B);
                    //    dpApprox.Add(dPoint);
                    //}

                    Approx = Fit.Polynomial(aX, aY, order, DirectRegressionMethod.QR);

                    foreach (DataPoint d in dp)
                    {
                        double y = 0;
                        for (int i = 0; i < Approx.Length; i++)
                        {
                            y += Approx[i] * Math.Pow(d.X, i);
                        }
                        DataPoint dPoint = new DataPoint(d.X, y);
                        dpApprox.Add(dPoint);
                    }

                    break;

                }
                catch
                {
                }
            }

            for(int i = Approx.Length - 1; i > 0; i-- )
            {
                if(!string.IsNullOrEmpty(Function))
                    Function += Approx[i] < 0 ? " - " : " + ";
                if(Math.Abs(Approx[i]) > 0.001)
                    Function += Math.Abs(Approx[i]).ToString("F2") + "x";
                if(i > 1 && !string.IsNullOrEmpty(Function))
                    Function += "^" + i.ToString();
            }
            if (!string.IsNullOrEmpty(Function))
                Function += Approx[0] < 0 ? " - " : " + ";
            Function += Math.Abs(Approx[0]).ToString("F2"); 


            if (kind == KindLineApprox.KOEFF)
            {
                ApproxA = Approx;
                KoeffFunc = Function;
            }
            else
            {
                ApproxR = Approx;
                ResistFunc = Function;
            }

            double StopX = dpApprox[dpApprox.Count - 1].X;
            //EndRange = dpApprox.Count - 1;

            // если нужно дополнить прогнозное построение
            while (EndX > StopX)
            {
                double y = 0;
                for (int i = 0; i < Approx.Length; i++)
                {
                    y += Approx[i] * Math.Pow(StopX, i);
                }
                DataPoint dPoint = new DataPoint(StopX, y);
                dpApprox.Add(dPoint);
                StopX += Step;
            }


            return dpApprox;
        }

    }
}