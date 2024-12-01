using App.MAUI.ViewModels;

namespace App.MAUI.Views;

public partial class ShopHomeView : ContentPage
{
	public ShopHomeView()
	{
		InitializeComponent();
		BindingContext = new ProductManagementViewModel();
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
		((ProductManagementViewModel)BindingContext).RefreshCartItems();
        ((ProductManagementViewModel)BindingContext).RefreshProducts();
    }

    private void ShopHome_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        ((ProductManagementViewModel)BindingContext).RefreshCartItems();
        ((ProductManagementViewModel)BindingContext).RefreshProducts();
    }
}