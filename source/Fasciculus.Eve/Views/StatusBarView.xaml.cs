using Fasciculus.Eve.ViewModels;

namespace Fasciculus.Eve.Views;

public partial class StatusBarView : ContentView
{
    public StatusBarView()
    {
        InitializeComponent();

        BindingContext = ServiceRegistry.GetRequiredService<StatusBarViewModel>();
    }
}