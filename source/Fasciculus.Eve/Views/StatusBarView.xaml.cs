using Fasciculus.Eve.ViewModels;
using Fasciculus.Maui.Support;

namespace Fasciculus.Eve.Views;

public partial class StatusBarView : ContentView
{
    public StatusBarView()
    {
        InitializeComponent();

        BindingContext = MauiAppServices.GetRequiredService<StatusBarViewModel>();
    }
}