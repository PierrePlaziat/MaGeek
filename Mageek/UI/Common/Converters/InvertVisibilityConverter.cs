using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Plaziat.CommonWpf
{
    public class InvertVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility v = (Visibility)value;
            if (v==Visibility.Visible) return Visibility.Collapsed;
            else if (v==Visibility.Collapsed) return Visibility.Visible;
            else if (v==Visibility.Hidden) return Visibility.Visible;
            else return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility v = (Visibility)value;
            if (v == Visibility.Visible) return Visibility.Collapsed;
            else if (v == Visibility.Collapsed) return Visibility.Visible;
            else if (v == Visibility.Hidden) return Visibility.Visible;
            else return Visibility.Collapsed;
        }
    }
}
