using Fasciculus.Eve.ViewModels;
using Fasciculus.Extensions;

namespace Fasciculus.Eve.Views;

public partial class NavBarView : ContentView
{
    public NavBarView()
    {
        InitializeComponent();

        BindingContext = ServiceRegistry.GetRequiredService<NavBarViewModel>();
    }
}