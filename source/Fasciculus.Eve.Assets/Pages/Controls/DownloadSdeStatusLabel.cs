using Fasciculus.Eve.Assets.Services;

namespace Fasciculus.Eve.Assets.Pages.Controls
{
    public partial class DownloadSdeStatusLabel : Label
    {
        public static readonly BindableProperty SourceProperty
            = BindableProperty.Create("Source", typeof(DownloadSdeStatus), typeof(Label), DownloadSdeStatus.Pending,
                BindingMode.OneWay, null, OnSourcePropertyChanged);

        private static void OnSourcePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            SetTextAndColor((DownloadSdeStatusLabel)bindable, (DownloadSdeStatus)newvalue);
        }

        private static void SetTextAndColor(DownloadSdeStatusLabel label, DownloadSdeStatus status)
        {
            label.Text = status switch
            {
                DownloadSdeStatus.Pending => "Pending",
                DownloadSdeStatus.Downloading => "Downloading",
                DownloadSdeStatus.Downloaded => "Downloaded",
                DownloadSdeStatus.NotModified => "Not Modified",
                _ => string.Empty
            };

            label.TextColor = status switch
            {
                DownloadSdeStatus.Pending => Colors.Red,
                DownloadSdeStatus.Downloading => Colors.Orange,
                DownloadSdeStatus.Downloaded => Colors.LightGreen,
                DownloadSdeStatus.NotModified => Colors.LightGreen,
                _ => Colors.Black
            };
        }

        public DownloadSdeStatus Source
        {
            get => (DownloadSdeStatus)GetValue(SourceProperty);
            set { SetValue(SourceProperty, value); }
        }

        public DownloadSdeStatusLabel()
        {
            SetTextAndColor(this, DownloadSdeStatus.Pending);
        }
    }
}
