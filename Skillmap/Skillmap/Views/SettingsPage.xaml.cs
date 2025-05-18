namespace Skillmap.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
	}

    private async void OnSaveSettingsClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Configuración", "Los ajustes han sido guardados.", "OK");
        await Navigation.PopAsync();
    }
}