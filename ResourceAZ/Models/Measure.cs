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
        private double _Current;
        public double Current
        {
            get => _Current;
            set
            {
                _Current = value;
                Koeff = _SummPot * _Current;
                if (_Current > 0)
                    Resist = _Napr / _Current;
            }

        }
        private double _Napr;
        public double Napr
        {
            get => _Napr;
            set
            {
                _Napr = value;
                if(_Current > 0)
                    Resist = _Napr / _Current;
            }
        }
        private double _SummPot;
        public double SummPot
        {
            get => _SummPot;
            set
            {
                _SummPot = value;
                Koeff = _SummPot * _Current;
            }
        }
        public double Koeff { get; set; }
        public double Resist { get; set; }
        public double ApprKoeff { get; set; }
        public double ApprResist { get; set; }
    }

}
