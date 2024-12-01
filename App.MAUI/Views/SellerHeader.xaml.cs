namespace App.MAUI.Views;

public partial class SellerHeader : VerticalStackLayout
{
	public SellerHeader()
	{
		InitializeComponent();
	}

    private void ImageButton_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//MainPage");
    }
}