using MathNet.Numerics;
using MathNet.Numerics.LinearRegression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceAZ.Chart
{
    class ApproxLine
    {
        private double[] _x;
        private double[] _y;
        public double[] ApproxDigit;
        public int endIndex;

        public double[] CalcDataPoint(double[] y, double[] x, int orderCalc = 3 )
        {
            if (x.Length <= 1)
                return null;

            endIndex = y.Length - 1;

            _y = new double[y.Length];
            _x = new double[x.Length];
            x.CopyTo(_x, 0);

            for (int order = orderCalc; order > 0; order--)
            {
                try
                {
                    ApproxDigit = Fit.Polynomial(x, y, order, DirectRegressionMethod.QR);

                    for (int index = 0; index < x.Length; index++)
                    {
                        double value = 0;
                        for (int i = 0; i < ApproxDigit.Length; i++)
                        {
                            value += ApproxDigit[i] * Math.Pow(x[index], i);
                        }
                        _y[index] = value;
                    }
                    break;

                }
                catch
                {
                }
            }

            return _y;
        }

        public string GetStringFunction()
        {
            // формирование строки функции
            string Function = "";
            for (int i = ApproxDigit.Length - 1; i > 0; i--)
            {
                if (!string.IsNullOrEmpty(Function))
                    Function += ApproxDigit[i] < 0 ? " - " : " + ";
                if (Math.Abs(ApproxDigit[i]) > 0.001)
                    Function += Math.Abs(ApproxDigit[i]).ToString("F2") + "x";
                if (i > 1 && !string.IsNullOrEmpty(Function))
                    Function += "^" + i.ToString();
            }
            if (!string.IsNullOrEmpty(Function))
                Function += ApproxDigit[0] < 0 ? " - " : " + ";
            Function += Math.Abs(ApproxDigit[0]).ToString("F2");

            return Function;
        }


        public bool CalcAddRange(double StopX, int Step = 10)
        {
            double EndX = _x.Last() + Step;
            int ptr = _x.Length;
            int addsValue = (int)(StopX - EndX) / Step + _x.Length + 1;
            if (addsValue < 1)
                return false;

            double[] newX = new double[addsValue];
            Array.Copy(_x, newX, _x.Length);
            _x = newX;

            double[] newY = new double[addsValue];
            Array.Copy(_y, newY, _y.Length);
            _y = newY;

            while (EndX <= StopX)
            {
                double y = 0;
                for (int i = 0; i < ApproxDigit.Length; i++)
                {
                    y += ApproxDigit[i] * Math.Pow(EndX, i);
                }
                ;
                _y[ptr] = y;
                _x[ptr] = EndX;
                EndX += Step;
                ptr++;
            }

            return true;
        }

        public double[] GetX() => _x;
        public double[] GetY() => _y;


    }
}
