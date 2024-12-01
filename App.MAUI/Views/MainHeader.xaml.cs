namespace App.MAUI.Views;

public partial class MainHeader : VerticalStackLayout
{
	public MainHeader()
	{
		InitializeComponent();
	}

    private void ImageButton_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//MainPage");
    }
}