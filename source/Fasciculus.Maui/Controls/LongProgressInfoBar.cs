using Fasciculus.Support;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Fasciculus.Maui.Controls
{
    public class LongProgressInfoBar : ProgressBar
    {
        public static readonly BindableProperty SourceProperty
            = BindableProperty.Create(nameof(Source), typeof(LongProgressInfo), typeof(LongProgressInfoBar), null,
                BindingMode.OneWay, null, OnSourcePropertyChanged);

        public static readonly BindableProperty BusyColorProperty
            = BindableProperty.Create(nameof(BusyColor), typeof(Color), typeof(LongProgressInfoBar), null,
                BindingMode.OneWay, null, null);

        public static readonly BindableProperty DoneColorProperty
            = BindableProperty.Create(nameof(DoneColor), typeof(Color), typeof(LongProgressInfoBar), null,
                BindingMode.OneWay, null, null);

        private static void OnSourcePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is LongProgressInfoBar progressBar)
            {
                if (newvalue is LongProgressInfo progressInfo)
                {
                    progressBar.Progress = progressInfo.Value;

                    if (progressInfo.Done)
                    {
                        if (progressBar.DoneColor is not null)
                        {
                            progressBar.ProgressColor = progressBar.DoneColor;
                        }
                    }
                    else
                    {
                        if (progressBar.BusyColor is not null)
                        {
                            progressBar.ProgressColor = progressBar.BusyColor;
                        }
                    }
                }
            }
        }

        public LongProgressInfo Source
        {
            get => (LongProgressInfo)GetValue(SourceProperty);
            set { SetValue(SourceProperty, value); }
        }

        public Color BusyColor
        {
            get => (Color)GetValue(BusyColorProperty);
            set { SetValue(BusyColorProperty, value); }
        }

        public Color DoneColor
        {
            get => (Color)GetValue(DoneColorProperty);
            set { SetValue(DoneColorProperty, value); }
        }
    }
}
