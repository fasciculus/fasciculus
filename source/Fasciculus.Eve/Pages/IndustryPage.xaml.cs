using Fasciculus.Eve.PageModels;

namespace Fasciculus.Eve.Pages;

public partial class IndustryPage : ContentPage
{
    public IndustryPage(IndustryPageModel model)
    {
        InitializeComponent();

        BindingContext = model;
    }
}