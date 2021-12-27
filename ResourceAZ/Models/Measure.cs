using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ResourceAZ.Models
{
    internal class Measure : INotifyPropertyChanged
    {
        DateTime _date;
        public DateTime date
        {
            get => _date;
            set {
                _date = value;
                OnPropertyChanged();
            }
        }
        public double Current { get; set; }
        public double Napr { get; set; }
        public double SummPot { get; set; }
        public double Koeff { get; set; }
        public double Resist { get; set; }
        public double ApprKoeff { get; set; }
        public double ApprResist { get; set; }
        bool _SetColor;
        public bool SetColor
        {
            get => _SetColor;
            set { _SetColor = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

    }

}
