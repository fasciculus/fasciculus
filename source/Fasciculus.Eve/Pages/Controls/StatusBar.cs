using Fasciculus.Eve.PageModels;
using System.ComponentModel;

namespace Fasciculus.Eve.Pages.Controls
{
    public partial class StatusBar : Label
    {
        public static readonly BindableProperty SourceProperty
            = BindableProperty.Create(nameof(Source), typeof(StatusBarModel), typeof(StatusBar), null,
                BindingMode.OneWay, null, OnSourceChanged);

        public static readonly BindableProperty ReadyColorProperty
            = BindableProperty.Create(nameof(ReadyColor), typeof(Color), typeof(StatusBar), null,
                BindingMode.OneWay, null, OnReadyColorChanged);

        public static readonly BindableProperty ErrorColorProperty
            = BindableProperty.Create(nameof(ErrorColor), typeof(Color), typeof(StatusBar), null,
                BindingMode.OneWay, null, OnErrorColorChanged);

        public StatusBarModel Source
        {
            get => (StatusBarModel)GetValue(SourceProperty);
            set { SetValue(SourceProperty, value); }
        }

        public Color ReadyColor
        {
            get => (Color)GetValue(ReadyColorProperty);
            set { SetValue(ReadyColorProperty, value); }
        }

        public Color ErrorColor
        {
            get => (Color)GetValue(ErrorColorProperty);
            set { SetValue(ErrorColorProperty, value); }
        }

        public StatusBar()
        {
            SetTextAndColor();
        }

        private void OnSourceValueChanged(object? sender, PropertyChangedEventArgs ev)
        {
            SetTextAndColor();
        }

        private static void OnSourceChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is StatusBar statusBar)
            {
                if (oldvalue is not null && oldvalue is StatusBarModel oldModel)
                {
                    oldModel.PropertyChanged -= statusBar.OnSourceValueChanged;
                }

                if (newvalue is not null && newvalue is StatusBarModel newModel)
                {
                    newModel.PropertyChanged += statusBar.OnSourceValueChanged;
                    statusBar.SetTextAndColor();
                }
            }
        }

        private static void OnReadyColorChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is StatusBar statusBar)
            {
                statusBar?.SetTextAndColor();
            }
        }

        private static void OnErrorColorChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is StatusBar statusBar)
            {
                statusBar?.SetTextAndColor();
            }
        }

        private void SetTextAndColor()
        {
            StatusBarModel? model = Source;

            if (model is not null)
            {
                Text = model.Text;

                if (model.IsError)
                {
                    if (ErrorColor is not null)
                    {
                        BackgroundColor = ErrorColor;
                    }
                }
                else
                {
                    if (ReadyColor is not null)
                    {
                        BackgroundColor = ReadyColor;
                    }
                }
            }
        }
    }
}
