using Fasciculus.Eve.PageModels;

namespace Fasciculus.Eve.Pages;

public partial class LoadingPage : ContentPage
{
    private readonly LoadingPageModel model;

    public LoadingPage(LoadingPageModel model)
    {
        InitializeComponent();

        BindingContext = this.model = model;
    }

    private void OnLoaded(object sender, EventArgs e)
    {
        model.OnLoaded();
    }
}