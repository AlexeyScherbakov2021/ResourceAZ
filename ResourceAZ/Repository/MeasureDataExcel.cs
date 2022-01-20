using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ClosedXML.Excel;
using ResourceAZ.Models;



namespace ResourceAZ.Repository
{
    class MeasureDataExcel : IMeasureData
    {
        public ObservableCollection<Measure> GetAllData(string Source)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            ObservableCollection<Measure> listM = new ObservableCollection<Measure>();

            using (XLWorkbook wb = new XLWorkbook(Source))
            {
                var ws = wb.Worksheets.Worksheet(1);

                for(int i = 2; i < 100000; i++)
                {

                    DateTime dt = ToDateTime(ws.Cell("A" + i).Value);

                    if (dt == DateTime.MinValue)
                        break;

                    Measure measure = new Measure();
                    measure.date = dt;

                    measure.Napr = ToDouble(ws.Cell("B" + i).Value);
                    measure.Current = ToDouble(ws.Cell("C" + i).Value);
                    measure.SummPot = ToDouble(ws.Cell("D" + i).Value);

                    //if (double.IsNaN(measure.SummPot))
                    //    measure.SummPot = 0;

                    if (measure.Napr <= 0 || measure.Current <= 0 || measure.SummPot == 0 ||
                        Double.IsNaN(measure.Napr) || Double.IsNaN(measure.Current) ||  Double.IsNaN(measure.SummPot))
                        continue;

                    listM.Add(measure);
                } 
            }

            Mouse.OverrideCursor = null;
            return listM;
        }


        public async void GetAllDataAsync(string Source, ObservableCollection<Measure> list)
        {
            App.CurrentAFWindow.Cursor = Cursors.Wait;
            Task task = GetList(Source, list);
            await task;
            App.CurrentAFWindow.Cursor = null;

        }

        private Task GetList(string Source, ObservableCollection<Measure> list)
        {
            return Task.Run(() =>
            {


                list.Clear();

                using (XLWorkbook wb = new XLWorkbook(Source))
                {
                    var ws = wb.Worksheets.Worksheet(1);

                    for (int i = 2; i < 100000; i++)
                    {

                        DateTime dt = ToDateTime(ws.Cell("A" + i).Value);

                        if (dt == DateTime.MinValue)
                            break;

                        Measure measure = new Measure();
                        measure.date = dt;

                        measure.Napr = ToDouble(ws.Cell("B" + i).Value);
                        measure.Current = ToDouble(ws.Cell("C" + i).Value);
                        measure.SummPot = ToDouble(ws.Cell("D" + i).Value);

                        if (measure.Napr <= 0 || measure.Current <= 0 /*|| measure.SummPot == 0*/ ||
                            Double.IsNaN(measure.Napr) || Double.IsNaN(measure.Current) || Double.IsNaN(measure.SummPot))
                            continue;

                        list.Add(measure);

                    }

                }

                
            });
        }






        double ToDouble(object obj)
        {
            //double d = 0;
            string s = obj.ToString();
            int index = s.IndexOfAny(new char[] {',', '.' });

            if (index < 1)
                return double.NaN;

            if (!double.TryParse(s, NumberStyles.Number, 
                new NumberFormatInfo() { NumberDecimalSeparator = s[index].ToString() } , 
                out double d))
                return double.NaN;

            //string ds = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            //string oldDecimalSeparator = ds.Equals(",") ? "." : ",";
            //s = s.Replace(oldDecimalSeparator, ds);

            //if (!Double.TryParse(s, out d))
            //    d = Double.NaN;

            return d;
        }

        DateTime ToDateTime(object obj)
        {
            DateTime dt = DateTime.MinValue;
            char[] sep;

            string s = obj.ToString();

            string[] datetime = s.Split(new char[] { ' ' });
            if (datetime.Count() == 2)
            {
                string date = datetime[0];
                string[] listUnitDate = date.Split(new char[] { '.', '/' });
                string time = datetime[1];
                string[] listUnitTime = time.Split(new char[] { ':', '.', '-' });

                date = listUnitDate[0] + "." + listUnitDate[1] + "." + listUnitDate[2] + " " +
                    listUnitTime[0] + ":" + listUnitTime[1] + ":00";
                if(!DateTime.TryParse( date, out dt))
                    return DateTime.MinValue;

            }

            DateTime.TryParse(obj.ToString(), out dt);

            return dt;
        }

    }
}
