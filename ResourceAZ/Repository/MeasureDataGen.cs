using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResourceAZ.Models;

namespace ResourceAZ.Repository
{
    internal class MeasureDataGen : IMeasureData
    {
        public ObservableCollection<Measure> GetAllData()
        {
            ObservableCollection<Measure> listMeasure = new ObservableCollection<Measure>()
            {
                new Measure { date = new DateTime(2014,1,1), Current=2.97, Napr=12.1, SummPot=-0.9 },
                new Measure { date = new DateTime(2015,1,1), Current=3.1, Napr=12.46, SummPot=-0.9},
                new Measure { date = new DateTime(2016,1,1), Current=3.25, Napr=12.87, SummPot=-0.9 },
                new Measure { date = new DateTime(2017,1,1), Current=3.4, Napr=13.26, SummPot=-0.9},
                new Measure { date = new DateTime(2018,1,1), Current=3.58, Napr=13.8, SummPot=-0.9},

                new Measure { date = new DateTime(2018,1,10), Current=3.5, Napr=13.71, SummPot=-0.9},
                new Measure { date = new DateTime(2018,2,1), Current=3.5, Napr=13.68, SummPot=-0.9},
                new Measure { date = new DateTime(2018,2,10), Current=3.5, Napr=13.85, SummPot=-0.9},
                new Measure { date = new DateTime(2018,2,14, 11, 30, 0), Current=3.5, Napr=13.7, SummPot=-0.9},
                new Measure { date = new DateTime(2018,2,14, 13, 24,0), Current=3.5, Napr=13.9, SummPot=-0.9},
            };

            return listMeasure;
        }
    }
}
