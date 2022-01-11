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
        public ObservableCollection<Measure> GetAllData(string Source = null)
        {
            ObservableCollection<Measure> listMeasure = new ObservableCollection<Measure>()
            {
                new Measure { date = new DateTime(2014,1,1), Current=2.97, Napr=12.1, SummPot=-0.9 },
                new Measure { date = new DateTime(2015,1,1), Current=3.1, Napr=12.46, SummPot=-0.9},
                new Measure { date = new DateTime(2016,1,1), Current=3.25, Napr=12.87, SummPot=-0.9 },
                new Measure { date = new DateTime(2017,1,1), Current=3.4, Napr=13.26, SummPot=-0.9},
                new Measure { date = new DateTime(2018,1,1), Current=3.58, Napr=13.8, SummPot=-0.9},

                new Measure { date = new DateTime(2018,5,10), Current=3.6, Napr=13.89, SummPot=-0.9},
                new Measure { date = new DateTime(2018,10,1), Current=3.68, Napr=14.68, SummPot=-0.9},
                new Measure { date = new DateTime(2019,1,10), Current=3.75, Napr=14.9, SummPot=-0.9},
                new Measure { date = new DateTime(2019,2,14, 11, 30, 0), Current=3.8, Napr=15.1, SummPot=-0.9},
                new Measure { date = new DateTime(2019,6,14, 13, 24,0), Current=3.84, Napr=15.9, SummPot=-0.9},
                new Measure { date = new DateTime(2019,9,10), Current=3.89, Napr=16.2, SummPot=-0.9},
                new Measure { date = new DateTime(2019,11,1), Current=4.1, Napr=16.25, SummPot=-0.9},
                new Measure { date = new DateTime(2020,1,10), Current=4.3, Napr=16.34, SummPot=-0.9},
                new Measure { date = new DateTime(2020,4,14, 11, 30,0), Current=4.8, Napr=16.5, SummPot=-0.9},
                new Measure { date = new DateTime(2020,9,14, 13, 24,0), Current=5.2, Napr=16.8, SummPot=-0.9},
                new Measure { date = new DateTime(2021,1,14, 13, 24,0), Current=5.4, Napr=16.9, SummPot=-0.9},
                new Measure { date = new DateTime(2021,4,14, 13, 24,0), Current=5.5, Napr=17.2, SummPot=-0.9},
                new Measure { date = new DateTime(2021,8,14, 13, 24,0), Current=5.8, Napr=17.4, SummPot=-0.9},
                new Measure { date = new DateTime(2021,10,14, 13, 24,0), Current=6.2, Napr=17.8, SummPot=-0.9},
            };

            return listMeasure;
        }

    }
}
