using ResourceAZ.Models;
using ResourceAZ.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ResourceAZ.Infrastructure
{
    public class RowToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Brush))
            {
                return null;
            }


            MainWindowViewModel model = (MainWindowViewModel)App.Current.MainWindow.DataContext;

            //// RowType - тип элемента строки
            var date = (DateTime)value;
            if (date != null && date >= model.MinSelectedValue && date <= model.MaxSelectedValue)
                return  new SolidColorBrush(Color.FromArgb(90, 255, 255, 0));

            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
