using System.Globalization;

namespace LingvoCards.App.Converters
{
    internal class SelectionColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Color.Parse("White");
            }
            else
            {
                return Application.Current?.Resources["Secondary"] ?? Color.Parse("White");
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
