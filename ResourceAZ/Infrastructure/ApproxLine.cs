using OxyPlot;
using ResourceAZ.ViewModels.Base;
using System.Collections.ObjectModel;
using MathNet.Numerics.LinearRegression;
using MathNet.Numerics.LinearAlgebra;

namespace ResourceAZ.ViewModels
{
    internal partial class MainWindowViewModel : ViewModel
    {

        private ObservableCollection<DataPoint> CalcDataPoint(ObservableCollection<DataPoint> dp)
        {
            if (dp.Count == 0)
                return null;

            ObservableCollection<DataPoint> dpApprox = new ObservableCollection<DataPoint>();

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

            try
            {
                var AB = SimpleRegression.Fit(aX, aY);

                double A = AB.Item2;
                double B = AB.Item1;

                foreach (DataPoint d in dp)
                {
                    DataPoint dPoint = new DataPoint(d.X, d.X * A + B);
                    dpApprox.Add(dPoint);
                }
            }
            catch { }

            return dpApprox;
        }

    }
}