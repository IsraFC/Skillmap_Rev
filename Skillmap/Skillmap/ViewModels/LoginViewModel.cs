using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skillmap.Services;
using System.Windows.Input;

namespace Skillmap.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly HttpService _httpService;

        [ObservableProperty] private string email = "admin@mail.com";
        [ObservableProperty] private string password = "Adm1234#";

        [ObservableProperty]
        private bool isAdmin;

        public ICommand LoginCommand => new AsyncRelayCommand(IniciarSesion);

        public LoginViewModel()
        {
            _httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
            VerificarRol();
        }

        private async void VerificarRol()
        {
            var rol = await SecureStorage.GetAsync("userRole");
            isAdmin = rol == "Admin";
        }

        private async Task IniciarSesion()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Por favor ingresa tu correo y contraseña", "OK");
                return;
            }

            bool exito = await _httpService.InitializeClient(Email.Trim(), Password);

            if (exito)
            {
                await App.Current.MainPage.DisplayAlert("Éxito", "Inicio de sesión exitoso", "OK");
                if (Application.Current is App app)
                    app.GoToMainApp();
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "Credenciales incorrectas", "OK");
            }
        }
    }
}
