using Fasciculus.Maui.Support;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Fasciculus.Maui.Controls
{
    public class WorkStateLabel : Label
    {
        public static readonly BindableProperty SourceProperty
            = BindableProperty.Create(nameof(Source), typeof(WorkState), typeof(WorkStateLabel), null,
                BindingMode.OneWay, null, OnWorkStateLabelPropertyChanged);

        public static readonly BindableProperty PendingColorProperty
            = BindableProperty.Create(nameof(PendingColor), typeof(Color), typeof(WorkStateLabel), null,
                BindingMode.OneWay, null, OnWorkStateLabelPropertyChanged);

        public static readonly BindableProperty WorkingColorProperty
            = BindableProperty.Create(nameof(WorkingColor), typeof(Color), typeof(WorkStateLabel), null,
                BindingMode.OneWay, null, OnWorkStateLabelPropertyChanged);

        public static readonly BindableProperty DoneColorProperty
            = BindableProperty.Create(nameof(DoneColor), typeof(Color), typeof(WorkStateLabel), null,
                BindingMode.OneWay, null, OnWorkStateLabelPropertyChanged);

        private static void OnWorkStateLabelPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is WorkStateLabel label)
            {
                label.SetTextAndColor();
            }
        }

        public WorkState? Source
        {
            get => (WorkState)GetValue(SourceProperty);
            set { SetValue(SourceProperty, value); }
        }

        public Color? PendingColor
        {
            get => (Color)GetValue(PendingColorProperty);
            set { SetValue(PendingColorProperty, value); }
        }

        public Color? WorkingColor
        {
            get => (Color)GetValue(WorkingColorProperty);
            set { SetValue(WorkingColorProperty, value); }
        }

        public Color? DoneColor
        {
            get => (Color)GetValue(DoneColorProperty);
            set { SetValue(DoneColorProperty, value); }
        }

        public WorkStateLabel()
        {
            SetTextAndColor();
        }

        private void SetTextAndColor()
        {
            SetText();
            SetTextColor();
        }

        private void SetText()
        {
            Text = Source switch
            {
                WorkState.Pending => "Pending",
                WorkState.Working => "Working",
                WorkState.Done => "Done",
                _ => string.Empty
            };
        }

        private void SetTextColor()
        {
            TextColor = Source switch
            {
                WorkState.Pending => PendingColor ?? Colors.Red,
                WorkState.Working => WorkingColor ?? Colors.Yellow,
                WorkState.Done => DoneColor ?? Colors.Green,
                _ => Colors.Transparent
            };
        }
    }
}
