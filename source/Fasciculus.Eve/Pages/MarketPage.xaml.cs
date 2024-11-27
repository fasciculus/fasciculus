using Fasciculus.Eve.PageModels;

namespace Fasciculus.Eve.Pages;

public partial class MarketPage : ContentPage
{
    public MarketPage(MarketPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}