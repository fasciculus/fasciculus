using Fasciculus.Support;
using System.Globalization;

namespace Fasciculus.Eve.Pages
{
    public class LongProgressInfoConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is LongProgressInfo info)
            {
                if (targetType == typeof(double))
                {
                    return info.Value;
                }

                if (targetType == typeof(Color))
                {
                    return info.Done ? Colors.LightGreen : Colors.Orange;
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
