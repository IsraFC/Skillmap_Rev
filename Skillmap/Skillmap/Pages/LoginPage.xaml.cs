namespace Skillmap.Pages;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(emailEntry.Text) && !string.IsNullOrEmpty(passwordEntry.Text))
        {
            await DisplayAlert("Bienvenido", "Inicio de sesi�n exitoso", "OK");

            // Cambia a la pantalla principal despu�s del login
            if (Application.Current is App app)
            {
                app.GoToMainApp();
            }
        }
        else
        {
            await DisplayAlert("Error", "Por favor ingresa tu correo y contrase�a", "OK");
        }
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Registro", "Redirigiendo a la p�gina de registro...", "OK");
    }

    private async void OnForgotPasswordTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert("Recuperaci�n de contrase�a", "Funcionalidad en desarrollo", "OK");
    }
}