namespace Skillmap.Views;

public partial class ProfilePage : ContentPage
{
	/// <summary>
	/// Default constructor
	/// </summary>
	public ProfilePage()
	{
		InitializeComponent();
	}

    private async void OnEditProfileClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditProfilePage());
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SettingsPage());
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Cerrar Sesi�n", "�Est�s seguro de que quieres cerrar sesi�n?", "S�", "No");
        if (confirm)
        {
            var loginPage = App.Current.Handler.MauiContext.Services.GetRequiredService<LoginPage>();
            Application.Current.MainPage = new NavigationPage(loginPage);
        }
    }
}