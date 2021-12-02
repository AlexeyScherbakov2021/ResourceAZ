using ResourceAZ.Models;
using ResourceAZ.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceAZ.ViewModels
{
    internal partial class CalcWindowViewModel : ViewModel
    {
        // переданные данные из исходных данных
        private KindGroup SelectGroup;
        private KindCalc SelectCalc;
        public double MinSummPot { get; set; }
        private ObservableCollection<Measure> InputMeasure;

        // рассчитанные данные
        private ObservableCollection<Measure> _listMeasure;
        public ObservableCollection<Measure> listMeasure
        {
            get => _listMeasure;
            set
            {
                Set(ref _listMeasure, value);
                //ModelToChart(listMeasure);
            }
        }
        


        public CalcWindowViewModel()
        {
            listMeasure = new ObservableCollection<Measure>();
        }

        public CalcWindowViewModel(KindGroup kg, KindCalc kc,double MinPot, ObservableCollection<Measure> measure) : this()
        {
            SelectGroup = kg;
            SelectCalc = kc;
            InputMeasure = measure;
            MinSummPot = MinPot;
        }
    }
}
