using Fasciculus.Eve.Support;

namespace Fasciculus.Eve.Controls
{
    public partial class IskLabel : Label
    {
        public static readonly BindableProperty ValueProperty
            = BindableProperty.Create(nameof(Value), typeof(double), typeof(IskLabel), null,
                BindingMode.OneWay, null, OnValuePropertyChanged);

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set { SetValue(ValueProperty, value); }
        }

        public IskLabel()
        {
            HorizontalTextAlignment = TextAlignment.End;

            UpdateText(this, 0);
        }

        private static void OnValuePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is IskLabel label)
            {
                if (newvalue is double value)
                {
                    UpdateText(label, value);
                }
            }
        }

        private static void UpdateText(IskLabel label, double value)
        {
            label.Text = value.ToString("#,###,###,##0", EveFormats.Isk);
        }
    }
}
