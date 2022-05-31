using System;
using System.Globalization;
using System.Windows.Data;

namespace Plaziat.CommonWpf
{
    public class NotConverter : IValueConverter
    {
        private NotConverter()
        {
        }

        public static NotConverter Instance { get; } = new NotConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = (bool)value;
            return !b;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = (bool)value;
            return !b;
        }
    }
}
