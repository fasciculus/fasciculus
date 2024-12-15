using Fasciculus.Eve.PageModels;

namespace Fasciculus.Eve.Pages;

public partial class SkillsPage : ContentPage
{
    public SkillsPage(SkillsPageModel model)
    {
        InitializeComponent();

        BindingContext = model;
    }
}