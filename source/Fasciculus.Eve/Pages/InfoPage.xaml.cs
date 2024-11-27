using Fasciculus.Eve.PageModels;

namespace Fasciculus.Eve.Pages;

public partial class InfoPage : ContentPage
{
    public InfoPage(InfoPageModel model)
    {
        InitializeComponent();

        BindingContext = model;
    }
}