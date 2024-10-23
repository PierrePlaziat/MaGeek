using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MageekDesktopClient.Business.Converters
{ 

    public class RarityToColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string rarity = value as string;
            switch (rarity)
            {
                case "mythic":
                    return new SolidColorBrush(Colors.OrangeRed);
                case "rare":
                    return new SolidColorBrush(Colors.Orange);
                case "uncommon":
                    return new SolidColorBrush(Colors.Gray);
                case "common":
                    return new SolidColorBrush(Colors.Black);
                case "bonus":
                    return new SolidColorBrush(Colors.MediumPurple);
                default:
                    return new SolidColorBrush(Colors.MediumOrchid);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

}
