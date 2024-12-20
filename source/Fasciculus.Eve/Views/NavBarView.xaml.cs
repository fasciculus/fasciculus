using Fasciculus.Eve.ViewModels;

namespace Fasciculus.Eve.Views;

public partial class NavBarView : ContentView
{
    public NavBarView()
    {
        InitializeComponent();

        BindingContext = ServiceRegistry.GetRequiredService<NavBarViewModel>();
    }
}