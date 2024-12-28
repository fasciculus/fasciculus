using System.Globalization;

namespace Fasciculus.Eve.Converters
{
    public class DoneToColor : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not null && value is bool done)
            {
                if (typeof(Color) == targetType)
                {
                    return done ? Colors.LightGreen : Color.FromArgb("FFD0D000");
                }
            }

            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
