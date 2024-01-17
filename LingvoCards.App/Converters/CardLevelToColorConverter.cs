using System.Globalization;
using LingvoCards.Domain.Model;

namespace LingvoCards.App.Converters
{
    public class CardLevelToColorConverter : IValueConverter
    {
        public static class CustomColors
        {
            public static Color BronzeFront = Color.FromArgb("#CD7F32");
            public static Color BronzeBack = Color.FromArgb("#703820");
            public static Color SilverFront = Color.FromArgb("#C0C0C0");
            public static Color SilverBack = Color.FromArgb("#BDC3C7");
            public static Color GoldFront = Color.FromArgb("#FFD700");
            public static Color GoldBack = Color.FromArgb("#B8860B");
            public static Color DiamondFront = Color.FromArgb("#B9F2FF");
            public static Color DiamondBack = Color.FromArgb("#3EA7D8");
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ELevel level)
            {
                return new SolidColorBrush(Colors.Transparent);
            }

            var isBack = parameter as string == "Back";
            return level switch
            {
                ELevel.Bronze => isBack ? CustomColors.BronzeBack : CustomColors.BronzeFront,
                ELevel.Silver => isBack ? CustomColors.SilverBack : CustomColors.SilverFront,
                ELevel.Gold => isBack ? CustomColors.GoldBack : CustomColors.GoldFront,
                ELevel.Diamond => isBack ? CustomColors.DiamondBack : CustomColors.DiamondFront,
                _ => Colors.Transparent,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Optional: Implement if needed
            throw new NotImplementedException();
        }
    }
}
