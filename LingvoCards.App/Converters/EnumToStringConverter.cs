using System.Globalization;

namespace LingvoCards.App.Converters
{
    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                if (Enum.TryParse(targetType, stringValue, out var result))
                {
                    return result;
                }
            }
            return null;
        }
    }
}
