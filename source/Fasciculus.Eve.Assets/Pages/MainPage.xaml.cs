namespace Fasciculus.Eve.Assets.Pages;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageModel model)
    {
        InitializeComponent();

        BindingContext = model;
    }
}