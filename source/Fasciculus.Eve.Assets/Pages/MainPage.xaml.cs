namespace Fasciculus.Eve.Assets.Pages;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageModel model)
    {
        InitializeComponent();

        BindingContext = model;

        model.CV1 = cv1;
        model.CV2 = cv2;
    }
}