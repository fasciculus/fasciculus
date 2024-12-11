using Fasciculus.Eve.PageModels;

namespace Fasciculus.Eve.Pages;

public partial class PlanetsPage : ContentPage
{
    public PlanetsPage(PlanetsPageModel model)
    {
        InitializeComponent();

        BindingContext = model;
    }
}