using App.MAUI.ViewModels;

namespace App.MAUI.Views;

public partial class ProductManagementView : ContentPage
{
	public ProductManagementView()
	{
		InitializeComponent();
		BindingContext = new ProductManagementViewModel();
	}

    private void Add_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//ProductEdit");
    }

    private void Edit_Clicked(object sender, EventArgs e)
    {
        ((ProductManagementViewModel)BindingContext).UpdateProduct();
    }

    private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        ((ProductManagementViewModel)BindingContext).RefreshProducts();
    }

    private void Delete_Clicked(object sender, EventArgs e)
    {
        ((ProductManagementViewModel)BindingContext).RefreshProducts();
    }
}