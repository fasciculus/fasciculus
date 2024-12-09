using Fasciculus.Maui.Support;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Fasciculus.Maui.Controls
{
    public class WorkStateLabel : Label
    {
        public static readonly BindableProperty SourceProperty
            = BindableProperty.Create(nameof(Source), typeof(WorkState), typeof(WorkStateLabel), null,
                BindingMode.OneWay, null, OnSourcePropertyChanged);

        public static readonly BindableProperty PendingColorProperty
            = BindableProperty.Create(nameof(PendingColor), typeof(Color), typeof(WorkStateLabel), null,
                BindingMode.OneWay, null, null);

        public static readonly BindableProperty WorkingColorProperty
            = BindableProperty.Create(nameof(WorkingColor), typeof(Color), typeof(WorkStateLabel), null,
                BindingMode.OneWay, null, null);

        public static readonly BindableProperty DoneColorProperty
            = BindableProperty.Create(nameof(DoneColor), typeof(Color), typeof(WorkStateLabel), null,
                BindingMode.OneWay, null, null);

        private static void OnSourcePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is WorkStateLabel label)
            {
                if (newvalue is WorkState state)
                {
                    SetTextAndColor(label, state);
                }
            }
        }

        private static void SetTextAndColor(WorkStateLabel label, WorkState state)
        {
            label.Text = state switch
            {
                WorkState.Pending => "Pending",
                WorkState.Working => "Working",
                WorkState.Done => "Done",
                _ => string.Empty
            };

            Color? color = state switch
            {
                WorkState.Pending => label.PendingColor,
                WorkState.Working => label.WorkingColor,
                WorkState.Done => label.DoneColor,
                _ => null
            };

            if (color is not null)
            {
                label.TextColor = color;
            }
        }

        public WorkState Source
        {
            get => (WorkState)GetValue(SourceProperty);
            set { SetValue(SourceProperty, value); }
        }

        public Color PendingColor
        {
            get => (Color)GetValue(PendingColorProperty);
            set { SetValue(PendingColorProperty, value); }
        }

        public Color WorkingColor
        {
            get => (Color)GetValue(WorkingColorProperty);
            set { SetValue(WorkingColorProperty, value); }
        }

        public Color DoneColor
        {
            get => (Color)GetValue(DoneColorProperty);
            set { SetValue(DoneColorProperty, value); }
        }

        public WorkStateLabel()
        {
            SetTextAndColor(this, WorkState.Pending);
        }
    }
}
