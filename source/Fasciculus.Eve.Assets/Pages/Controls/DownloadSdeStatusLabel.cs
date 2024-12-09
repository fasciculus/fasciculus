using Fasciculus.Eve.Assets.Services;

namespace Fasciculus.Eve.Assets.Pages.Controls
{
    public partial class DownloadSdeStatusLabel : Label
    {
        private static readonly Color PendingColor = Color.FromArgb("#FFFF6060");
        private static readonly Color WorkingColor = Color.FromArgb("#FFD0D000");

        public static readonly BindableProperty SourceProperty
            = BindableProperty.Create(nameof(Source), typeof(DownloadSdeStatus), typeof(Label), null,
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
                DownloadSdeStatus.Pending => PendingColor,
                DownloadSdeStatus.Downloading => WorkingColor,
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
