using Fasciculus.Eve.ViewModels;
using Fasciculus.Maui.Support;

namespace Fasciculus.Eve.Views;

public partial class NavBarView : ContentView
{
    public NavBarView()
    {
        InitializeComponent();

        BindingContext = MauiAppServices.GetRequiredService<NavBarViewModel>();
    }
}