using System.Globalization;

namespace LingvoCards.App.Converters;

internal class NullToPlaceholderConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return "<Not selected>"; // Or return String.Empty for an empty string
        }

        // Assuming the object has a 'Text' property you want to display
        return value.GetType().GetProperty("Text")?.GetValue(value) ?? String.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Implement as needed, e.g., return null when "All" is selected
        return Binding.DoNothing;
    }
}