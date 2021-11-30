using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceAZ.Models
{
    internal class Measure
    {
        public DateTime date { get; set; }
        public double Current { get; set; }
        public double Napr { get; set; }
        public double SummPot { get; set; }
        public double Koeff { get; set; }
        public double Resist { get; set; }
    }


}
