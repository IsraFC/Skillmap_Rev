namespace Skillmap.Pages;

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
        bool confirm = await DisplayAlert("Cerrar Sesión", "¿Estás seguro de que quieres cerrar sesión?", "Sí", "No");
        if (confirm)
        {
            // Reemplazar toda la pila de navegación con LoginPage
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}