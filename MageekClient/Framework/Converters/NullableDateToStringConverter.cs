using System;
using System.Windows.Data;

namespace MaGeek.Framework.Converters
{

    public class NullableDateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return "??/??/????";

            DateTime returnVal;

            if (DateTime.TryParse(value.ToString(), out returnVal))
            {
                if (returnVal != DateTime.MinValue)
                    return returnVal;
                else
                    return "??/??/????";
            }
            else
                return "??/??/????";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return DateTime.MinValue;

            DateTime val;
            if (value.ToString() == "??/??/????")
                return DateTime.MinValue;

            if (DateTime.TryParse(value.ToString(), out val))
                return val;
            else
                return DateTime.MinValue;
        }

    }
}
