using Fasciculus.Eve.PageModels;

namespace Fasciculus.Eve.Pages;

public partial class PlanetaryIndustryPage : ContentPage
{
    public PlanetaryIndustryPage(PlanetaryIndustryPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}