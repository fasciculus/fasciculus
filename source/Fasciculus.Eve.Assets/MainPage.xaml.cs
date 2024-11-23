namespace Fasciculus.Eve.Assets
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }

        private void OnStartClicked(object sender, EventArgs e)
        {
        }
    }
}
