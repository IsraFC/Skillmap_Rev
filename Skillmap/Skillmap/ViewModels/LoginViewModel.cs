using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skillmap.Services;
using System.Windows.Input;

namespace Skillmap.ViewModels
{
    /// <summary>
    /// ViewModel encargado del proceso de inicio de sesión en la aplicación.
    /// Administra los datos de acceso, validaciones y redirección tras el login.
    /// </summary>
    public partial class LoginViewModel : ObservableObject
    {
        private readonly HttpService _httpService;

        /// <summary>
        /// Correo electrónico ingresado por el usuario.
        /// </summary>
        [ObservableProperty] private string email = "admin@mail.com";

        /// <summary>
        /// Contraseña ingresada por el usuario.
        /// </summary>
        [ObservableProperty] private string password = "Adm1234#";

        /// <summary>
        /// Indica si el usuario autenticado tiene rol de administrador.
        /// </summary>
        [ObservableProperty] private bool isAdmin;

        /// <summary>
        /// Comando para ejecutar el proceso de inicio de sesión.
        /// </summary>
        public ICommand LoginCommand => new AsyncRelayCommand(IniciarSesion);

        /// <summary>
        /// Constructor que obtiene el servicio HTTP y verifica el rol guardado, si lo hay.
        /// </summary>
        public LoginViewModel()
        {
            _httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
            VerificarRol();
        }

        /// <summary>
        /// Verifica el rol almacenado en SecureStorage para identificar si es un administrador.
        /// </summary>
        private async void VerificarRol()
        {
            var rol = await SecureStorage.GetAsync("userRole");
            isAdmin = rol == "Admin";
        }

        /// <summary>
        /// Realiza la autenticación del usuario con el email y contraseña ingresados.
        /// Si es exitosa, redirige a la aplicación principal.
        /// </summary>
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

                // Redirige a la app principal si la autenticación fue exitosa
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
