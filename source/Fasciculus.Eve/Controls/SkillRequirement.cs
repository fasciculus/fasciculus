using Fasciculus.Eve.Models;

namespace Fasciculus.Eve.Controls
{
    public partial class SkillRequirement : HorizontalStackLayout
    {
        public static readonly BindableProperty SourceProperty
            = BindableProperty.Create(nameof(Source), typeof(EveSkillRequirement), typeof(SkillRequirement), null,
                BindingMode.OneWay, null, OnSourcePropertyChanged);

        private readonly Label nameLabel = new();

        private readonly HorizontalStackLayout levelLayout = new() { Spacing = 2, Padding = new Thickness(0, 3, 0, 0) };

        private readonly BoxView level1 = CreateLevelBox();
        private readonly BoxView level2 = CreateLevelBox();
        private readonly BoxView level3 = CreateLevelBox();
        private readonly BoxView level4 = CreateLevelBox();
        private readonly BoxView level5 = CreateLevelBox();

        public EveSkillRequirement? Source
        {
            get => (EveSkillRequirement)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public SkillRequirement()
        {
            levelLayout.Add(level1);
            levelLayout.Add(level2);
            levelLayout.Add(level3);
            levelLayout.Add(level4);
            levelLayout.Add(level5);

            Add(levelLayout);
            Add(nameLabel);

            Spacing = 14;
        }

        private static void OnSourcePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is SkillRequirement control)
            {
                if (newvalue is null)
                {
                    control.nameLabel.Text = "";
                    control.level1.Color = NotReachedColor;
                    control.level2.Color = NotReachedColor;
                    control.level3.Color = NotReachedColor;
                    control.level4.Color = NotReachedColor;
                    control.level5.Color = NotReachedColor;
                }

                if (newvalue is EveSkillRequirement requirement)
                {
                    control.nameLabel.Text = requirement.Name;
                    control.level1.Color = SelectColor(1, requirement.RequiredLevel, requirement.CurrentLevel);
                    control.level2.Color = SelectColor(2, requirement.RequiredLevel, requirement.CurrentLevel);
                    control.level3.Color = SelectColor(3, requirement.RequiredLevel, requirement.CurrentLevel);
                    control.level4.Color = SelectColor(4, requirement.RequiredLevel, requirement.CurrentLevel);
                    control.level5.Color = SelectColor(5, requirement.RequiredLevel, requirement.CurrentLevel);
                }
            }
        }

        private static BoxView CreateLevelBox()
            => new() { Color = NotReachedColor, WidthRequest = 10, HeightRequest = 10 };

        private static readonly Color ReachedColor = Colors.White;
        private static readonly Color NotReachedColor = Color.FromRgba("404040");
        private static readonly Color FulfilledColor = Colors.Green;
        private static readonly Color MissingColor = Colors.Red;

        private static Color SelectColor(int level, int required, int current)
        {
            if (level <= required)
            {
                return level <= current ? FulfilledColor : MissingColor;
            }
            else
            {
                return level <= current ? ReachedColor : NotReachedColor;
            }
        }
    }
}
