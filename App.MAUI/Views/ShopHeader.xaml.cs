using App.MAUI.ViewModels;

namespace App.MAUI.Views;

public partial class ShopHeader : VerticalStackLayout
{
	public ShopHeader()
	{
		InitializeComponent();
        BindingContext = new ProductManagementViewModel();
	}

    private void ImageButton_Clicked(object sender, EventArgs e)
    {
        MessagingCenter.Send<ProductManagementViewModel>(((ProductManagementViewModel)BindingContext), "ShopperSidePanelVisibilityUpdated");
    }

    private void CartButton_Clicked(object sender, EventArgs e)
    {
        MessagingCenter.Send<ProductManagementViewModel>(((ProductManagementViewModel)BindingContext), "CartVisibilityUpdated");
    }
}