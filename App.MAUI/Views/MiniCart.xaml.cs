using App.MAUI.ViewModels;
using Library.Models;
using Library.Services;

namespace App.MAUI.Views;

public partial class MiniCart : ContentView
{
	public MiniCart()
	{
		InitializeComponent();
		BindingContext = new ProductManagementViewModel();
        MiniCartView.TranslateTo(325, 0, 200);
        MessagingCenter.Subscribe<ProductManagementViewModel>(this, "CartVisibilityUpdated", (sender) =>
        {
            if (((ProductManagementViewModel)BindingContext).CartVisible == true)
            {
                HideCart();
            }
            else
            {
                ((ProductManagementViewModel)BindingContext).CartVisible = true;
                MiniCartView.TranslateTo(0, 0, 200);
            }
        });
    }

    private async void HideCart()
    {
        await MiniCartView.TranslateTo(325, 0, 200);
        ((ProductManagementViewModel)BindingContext).CartVisible = false;
    }

    private void DeleteFromCart_Clicked(object sender, EventArgs e)
    {
		((ProductManagementViewModel)BindingContext).RefreshCartItems();
        ((ProductManagementViewModel)BindingContext).RefreshProducts();
    }

    private void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        ((ProductManagementViewModel)BindingContext).RefreshCartItems();
        ((ProductManagementViewModel)BindingContext).RefreshProducts();
    }
}