using Fasciculus.Eve.Assets.Services;
using Fasciculus.Support;
using System.Globalization;

namespace Fasciculus.Eve.Assets.Pages
{
    public class DownloadSdeStatusConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DownloadSdeStatus status)
            {
                if (targetType == typeof(string))
                {
                    return status switch
                    {
                        DownloadSdeStatus.Pending => "Pending",
                        DownloadSdeStatus.Downloading => "Downloading",
                        DownloadSdeStatus.Downloaded => "Downloaded",
                        DownloadSdeStatus.NotModified => "Not Modified",
                        _ => string.Empty
                    };
                }

                if (targetType == typeof(Color))
                {
                    return status switch
                    {
                        DownloadSdeStatus.Pending => Colors.Orange,
                        DownloadSdeStatus.Downloading => Colors.Orange,
                        DownloadSdeStatus.Downloaded => Colors.Green,
                        DownloadSdeStatus.NotModified => Colors.Green,
                        _ => Colors.Black
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
                    return info.Done ? Colors.Green : Colors.Orange;
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
