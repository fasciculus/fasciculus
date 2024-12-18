using Fasciculus.Eve.PageModels;

namespace Fasciculus.Eve.Pages;

public partial class DebugPage : ContentPage
{
    public DebugPage(DebugPageModel model)
    {
        InitializeComponent();

        BindingContext = model;
    }
}