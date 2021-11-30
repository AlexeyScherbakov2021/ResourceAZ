using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResourceAZ.Models;

namespace ResourceAZ.Repository
{
    internal class MeasureDataGen : IMeasureData
    {
        public List<Measure> GetAllData()
        {
            List<Measure> listMeasure = new List<Measure>()
            {
                new Measure { date = new DateTime(2014,1,1), Current=2.97, Napr=12.1, SummPot=-0.9 },
                new Measure { date = new DateTime(2015,1,1), Current=3.1, Napr=12.46, SummPot=-0.9},
                new Measure { date = new DateTime(2016,1,1), Current=3.25, Napr=12.87, SummPot=-0.9 },
                new Measure { date = new DateTime(2017,1,1), Current=3.4, Napr=13.26, SummPot=-0.9},
                new Measure { date = new DateTime(2018,1,1), Current=3.58, Napr=13.8, SummPot=-0.9},
            };

            return listMeasure;
        }
    }
}
