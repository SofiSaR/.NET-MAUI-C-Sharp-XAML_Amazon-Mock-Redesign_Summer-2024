using App.MAUI.ViewModels;
using Library.Services;

namespace App.MAUI.Views;

[QueryProperty(nameof(ProductId), "productId")]

public partial class ProductEditView : ContentPage
{
    public int ProductId { get; set; }
    public ProductEditView()
	{
		InitializeComponent();
	}

    private void Cancel_Clicked(object sender, EventArgs e)
    {
		Shell.Current.GoToAsync("//ProductManagement");
    }

    private void Save_Clicked(object sender, EventArgs e)
    {
        if (ProductId == 0)
            ((ProductViewModel)BindingContext).Add();
        else ((ProductViewModel)BindingContext).Update();
        Shell.Current.GoToAsync("//ProductManagement");
    }

    private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        BindingContext = new ProductViewModel(ProductId);
    }
}