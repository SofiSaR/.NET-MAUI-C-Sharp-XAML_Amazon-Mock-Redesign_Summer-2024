using App.MAUI.ViewModels;

namespace App.MAUI.Views;

public partial class SellersSideMenu : HorizontalStackLayout
{
	public SellersSideMenu()
	{
		InitializeComponent();
        BindingContext = new SidePanelViewModel();
    }

    private async void ImageButton_Clicked(object sender, EventArgs e)
    {
        if (((SidePanelViewModel)BindingContext).PaneVisible == true)
		{
            await SidePanel.TranslateTo(-225, 0, 200);
            await Strip.TranslateTo(-107, 0, 5);
            ((SidePanelViewModel)BindingContext).PaneVisible = false;
            CollapseButton.TranslateTo(100, 0, 5);
            Strip.TranslateTo(-107, 0, 5);
            ((SidePanelViewModel)BindingContext).LeftMargin = 0;
        }
        else
        {
            await CollapseButton.TranslateTo(0, 0, 5);
            ((SidePanelViewModel)BindingContext).PaneVisible = true;
            SidePanel.TranslateTo(0, 0, 200);
            Strip.TranslateTo(0, 0, 5);
            ((SidePanelViewModel)BindingContext).LeftMargin = 257;
        }
        CollapseButton.ScaleTo(CollapseButton.Scale * -1, 10);
    }

    private void Logout_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//MainPage");
    }
    private void ProductNav_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//ProductManagement");
    }

    private void StoreSettingsNav_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//StoreSettings");
    }
}