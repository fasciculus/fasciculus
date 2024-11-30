using Fasciculus.Eve.Assets.Services;
using System.Globalization;

namespace Fasciculus.Eve.Assets.Pages
{
    public class PendingToDoneConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is PendingToDone status)
            {
                if (targetType == typeof(string))
                {
                    return status switch
                    {
                        PendingToDone.Pending => "Pending",
                        PendingToDone.Working => "Working",
                        PendingToDone.Done => "Done",
                        _ => string.Empty
                    };
                }

                if (targetType == typeof(Color))
                {
                    return status switch
                    {
                        PendingToDone.Pending => Colors.Orange,
                        PendingToDone.Working => Colors.Orange,
                        PendingToDone.Done => Colors.Green,
                        _ => Colors.Red
                    };
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
