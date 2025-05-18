using Skillmap.Services;
using Microsoft.Maui.Controls;

namespace Skillmap.Views;

public partial class LoginPage : ContentPage
{
    private readonly HttpService _httpService;

    public LoginPage(HttpService httpService)
	{
		InitializeComponent();
        _httpService = httpService;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string email = emailEntry.Text?.Trim() ?? "";
        string password = passwordEntry.Text ?? "";

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Error", "Por favor ingresa tu correo y contraseña", "OK");
            return;
        }

        var success = await _httpService.InitializeClient(email, password);

        if (success)
        {
            await DisplayAlert("Éxito", "Inicio de sesión exitoso", "OK");

            if (Application.Current is App app)
            {
                app.GoToMainApp(); // Navega al AppShell u otra página principal
            }
        }
        else
        {
            await DisplayAlert("Error", "Credenciales incorrectas", "OK");
        }
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Registro", "Redirigiendo a la página de registro...", "OK");
    }

    private async void OnForgotPasswordTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert("Recuperación de contraseña", "Funcionalidad en desarrollo", "OK");
    }
}