using Fasciculus.Eve.PageModels;

namespace Fasciculus.Eve.Pages;

public partial class MapPage : ContentPage
{
    public MapPage(MapPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}