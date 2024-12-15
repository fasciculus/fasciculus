using Fasciculus.Eve.PageModels;

namespace Fasciculus.Eve.Pages;

public partial class SkillsPage : ContentPage
{
    private readonly SkillsPageModel model;

    public SkillsPage(SkillsPageModel model)
    {
        InitializeComponent();

        BindingContext = this.model = model;
    }

    private void OnLoaded(object sender, EventArgs e)
    {
        model.OnLoaded();
    }
}