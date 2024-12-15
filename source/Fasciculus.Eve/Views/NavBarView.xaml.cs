using Fasciculus.Eve.ViewModels;
using Fasciculus.Support;

namespace Fasciculus.Eve.Views;

public partial class NavBarView : ContentView
{
    public NavBarView()
    {
        InitializeComponent();

        BindingContext = GlobalServices.GetRequiredService<NavBarViewModel>();
    }
}