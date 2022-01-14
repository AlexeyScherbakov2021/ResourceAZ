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

        //private double[] CalcDataPoint(double[] dp, double[] datesRange, KindLineApprox kind, double EndX = -1, double Step = 10)
        //{
        //    if (dp.Length <= 1)
        //        return null;

        //    double[] dpApprox = new double[dp.Length];
        //    double[] Approx = null;
        //    string Function = "";

        //    double X = 0;
        //    double X2 = 0;
        //    double Y = 0;
        //    double XY = 0;
        //    double N = datesRange.Length;

        //    for (int order = orderCalc; order > 0; order--)
        //    {

        //        try
        //        {
        //            //var AB = SimpleRegression.Fit(aX, aY);

        //            //double A = AB.Item2;
        //            //double B = AB.Item1;

        //            //foreach (DataPoint d in dp)
        //            //{
        //            //    DataPoint dPoint = new DataPoint(d.X, d.X * A + B);
        //            //    dpApprox.Add(dPoint);
        //            //}

        //            Approx = Fit.Polynomial(datesRange, dp, order, DirectRegressionMethod.QR);

        //            for(int index = 0; index < dpApprox.Length; index++)
        //            {
        //                double y = 0;
        //                for (int i = 0; i < Approx.Length; i++)
        //                {
        //                    y += Approx[i] * Math.Pow(dates[index], i);
        //                }
        //                //DataPoint dPoint = new DataPoint(d.X, y);
        //                dpApprox[index] = y;
        //            }

        //            break;

        //        }
        //        catch
        //        {
        //        }
        //    }

        //    for(int i = Approx.Length - 1; i > 0; i-- )
        //    {
        //        if(!string.IsNullOrEmpty(Function))
        //            Function += Approx[i] < 0 ? " - " : " + ";
        //        if(Math.Abs(Approx[i]) > 0.001)
        //            Function += Math.Abs(Approx[i]).ToString("F2") + "x";
        //        if(i > 1 && !string.IsNullOrEmpty(Function))
        //            Function += "^" + i.ToString();
        //    }
        //    if (!string.IsNullOrEmpty(Function))
        //        Function += Approx[0] < 0 ? " - " : " + ";
        //    Function += Math.Abs(Approx[0]).ToString("F2"); 


        //    if (kind == KindLineApprox.KOEFF)
        //    {
        //        ApproxA = Approx;
        //        KoeffFunc = Function;
        //    }
        //    else
        //    {
        //        ApproxR = Approx;
        //        ResistFunc = Function;
        //    }

        //    double StopX = dates[dates.Length - 1];
        //    //EndRange = dpApprox.Count - 1;

        //    // если нужно дополнить прогнозное построение
        //    //while (EndX > StopX)
        //    //{
        //    //    double y = 0;
        //    //    for (int i = 0; i < Approx.Length; i++)
        //    //    {
        //    //        y += Approx[i] * Math.Pow(StopX, i);
        //    //    }
        //    //    DataPoint dPoint = new DataPoint(StopX, y);
        //    //    dpApprox.Add(dPoint);
        //    //    StopX += Step;
        //    //}


        //    return dpApprox;
        //}

    }
}