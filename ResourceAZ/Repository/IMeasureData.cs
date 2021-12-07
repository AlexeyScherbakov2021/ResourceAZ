using ResourceAZ.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceAZ.Repository
{
    interface IMeasureData
    {
        ObservableCollection<Measure> GetAllData(string Source = null);

    }
}
