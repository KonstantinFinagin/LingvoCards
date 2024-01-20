using System.Globalization;

namespace LingvoCards.App.Converters
{
    public class IndexPlusOneConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index)
            {
                return index + 1;
            }
            return value; // Or handle non-int values as you see fit
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index)
            {
                return index - 1;
            }
            return value; // Or handle non-int values as you see fit
        }
    }
}
