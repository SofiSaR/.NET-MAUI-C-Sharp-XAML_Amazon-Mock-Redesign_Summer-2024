using App.MAUI.ViewModels;

namespace App.MAUI.Views;

public partial class ShopperSidePanel : ContentView
{
	public ShopperSidePanel()
	{
		InitializeComponent();
		BindingContext = new ProductManagementViewModel();
        ShopperSidePanelView.TranslateTo(325, 0, 200);
        MessagingCenter.Subscribe<ProductManagementViewModel>(this, "ShopperSidePanelVisibilityUpdated", (sender) =>
        {
            if (((ProductManagementViewModel)BindingContext).ShopperSidePanelVisible == true)
            {
                HideShopperSidePanel();
            }
            else
            {
                ((ProductManagementViewModel)BindingContext).ShopperSidePanelVisible = true;
                ShopperSidePanelView.TranslateTo(0, 0, 200);
            }
        });
    }

    private async void HideShopperSidePanel()
    {
        await ShopperSidePanelView.TranslateTo(325, 0, 200);
        ((ProductManagementViewModel)BindingContext).ShopperSidePanelVisible = false;
    }

    private void WishlistNav_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//Wishlists");
    }

    private void Logout_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//MainPage");
    }
}