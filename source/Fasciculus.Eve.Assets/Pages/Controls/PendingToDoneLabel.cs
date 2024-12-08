using Fasciculus.Eve.Assets.Services;

namespace Fasciculus.Eve.Assets.Pages.Controls
{
    public partial class PendingToDoneLabel : Label
    {
        public static readonly BindableProperty SourceProperty
            = BindableProperty.Create("Source", typeof(PendingToDone), typeof(Label), PendingToDone.Pending,
                BindingMode.OneWay, null, OnSourcePropertyChanged);

        private static void OnSourcePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            SetTextAndColor((PendingToDoneLabel)bindable, (PendingToDone)newvalue);
        }

        private static void SetTextAndColor(PendingToDoneLabel label, PendingToDone status)
        {
            label.Text = status switch
            {
                PendingToDone.Pending => "Pending",
                PendingToDone.Working => "Working",
                PendingToDone.Done => "Done",
                _ => string.Empty
            };

            label.TextColor = status switch
            {
                PendingToDone.Pending => Colors.Orange,
                PendingToDone.Working => Colors.Orange,
                PendingToDone.Done => Colors.Green,
                _ => Colors.Red
            };
        }

        public PendingToDone Source
        {
            get => (PendingToDone)GetValue(SourceProperty);
            set { SetValue(SourceProperty, value); }
        }

        public PendingToDoneLabel()
        {
            SetTextAndColor(this, PendingToDone.Pending);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
        }
    }
}
