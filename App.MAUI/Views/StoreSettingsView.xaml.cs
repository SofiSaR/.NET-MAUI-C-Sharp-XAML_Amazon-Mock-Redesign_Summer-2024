using App.MAUI.ViewModels;
using Library.Models;

namespace App.MAUI.Views;

public partial class StoreSettingsView : ContentPage
{
	public StoreSettingsView()
	{
		InitializeComponent();
        BindingContext = new ProductManagementViewModel();
    }
}