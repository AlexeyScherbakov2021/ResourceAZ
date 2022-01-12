using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceAZ.Chart
{
    interface IChart
    {

        void Create();
        void AddSeries(int Count);
        void NewData(double[] X, double[] Y);
        void NewData2(double[] X, double[] Y);

    }
}
