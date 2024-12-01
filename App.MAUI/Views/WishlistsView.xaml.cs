using App.MAUI.ViewModels;

namespace App.MAUI.Views;

public partial class WishlistsView : ContentPage
{
	public WishlistsView()
	{
		InitializeComponent();
		BindingContext = new WishlistsViewModel();
	}

    private void CreateWishlist(object sender, EventArgs e)
    {
        ((WishlistsViewModel)BindingContext).AddingWishlist = true;
    }

    private void AddWishlist(object sender, EventArgs e)
    {
        ((WishlistsViewModel)BindingContext).AddWishlist();
    }

    private void DeleteFromCart_Clicked(object sender, EventArgs e)
    {
        ((WishlistsViewModel)BindingContext).RefreshCarts();
    }

    private void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        ((WishlistsViewModel)BindingContext).RefreshCarts();
    }

    private void Wishlists_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        ((WishlistsViewModel)BindingContext).RefreshCarts();
    }
}