using App.MAUI.ViewModels;

namespace App.MAUI
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
        }

        private void ShopperClicked(object sender, EventArgs e)
        {
            ((MainViewModel)BindingContext).PageToGo = "//ShopHome";
            ((MainViewModel)BindingContext).ShopperButtonColor = (Color)Application.Current.Resources["Secondary"];
            ((MainViewModel)BindingContext).SellerButtonColor = (Color)Application.Current.Resources["Transparent"];
        }

        private void SellerClicked(object sender, EventArgs e)
        {
            ((MainViewModel)BindingContext).PageToGo = "//ProductManagement";
            ((MainViewModel)BindingContext).ShopperButtonColor = (Color)Application.Current.Resources["Transparent"];
            ((MainViewModel)BindingContext).SellerButtonColor = (Color)Application.Current.Resources["Secondary"];
        }

        private void ContinueClicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync(((MainViewModel)BindingContext).PageToGo);
        }
    }

}
