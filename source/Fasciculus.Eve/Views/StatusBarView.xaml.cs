using Fasciculus.Eve.ViewModels;
using Fasciculus.Support;

namespace Fasciculus.Eve.Views;

public partial class StatusBarView : ContentView
{
    public StatusBarView()
    {
        InitializeComponent();

        BindingContext = GlobalServices.GetRequiredService<StatusBarViewModel>();
    }
}