using System.ComponentModel;

namespace Fasciculus.Eve.Assets
{
    public partial class MainPage : ContentPage
    {
        private readonly MainPageModel model;

        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;

            this.model = model;

            // model.PropertyChanged += ModelPropertyChanged;
        }

        private void ModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            BindingContext = null;
            BindingContext = model;
        }
    }
}
