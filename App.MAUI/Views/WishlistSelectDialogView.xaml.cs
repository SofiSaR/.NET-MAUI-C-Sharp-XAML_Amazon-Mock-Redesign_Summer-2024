using App.MAUI.ViewModels;

namespace App.MAUI.Views;

using Product = Library.Models.Product;

public partial class WishlistSelectDialogView : Grid
{
	public WishlistSelectDialogView()
	{
		InitializeComponent();
		BindingContext = new WishlistsViewModel();
	}

    private void CreateWishlist(object sender, EventArgs e)
    {
        ((WishlistsViewModel)BindingContext).AddingWishlist = true;
    }

    private void AddToWishlists(object sender, EventArgs e)
    {
		((WishlistsViewModel)BindingContext).AddToCarts();
    }

    private void AddWishlist(object sender, EventArgs e)
    {
        ((WishlistsViewModel)BindingContext).AddWishlist();
    }


    private void Close_Clicked(object sender, EventArgs e)
    {
        ((WishlistsViewModel)BindingContext).ShowWishlistSelectDialog = false;
    }
}