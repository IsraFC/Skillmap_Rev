using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skillmap.Services;
using SkillmapLib1.Models.DTO.OutputDTO;

namespace Skillmap.ViewModels
{
    /// <summary>
    /// ViewModel que gestiona la información y acciones del perfil del usuario actual.
    /// Permite visualizar los datos, navegar a la edición del perfil, cerrar sesión o crear un nuevo usuario.
    /// </summary>
    public partial class ProfileViewModel : ObservableObject
    {
        /// <summary>
        /// Nombre de usuario del usuario autenticado.
        /// </summary>
        [ObservableProperty] private string userName = string.Empty;

        /// <summary>
        /// Nombre completo del usuario (nombre + apellidos).
        /// </summary>
        [ObservableProperty] private string nombreCompleto = string.Empty;

        /// <summary>
        /// Indica si el usuario tiene rol de administrador.
        /// </summary>
        [ObservableProperty] private bool esAdmin;

        private readonly HttpService _httpService;

        /// <summary>
        /// Constructor que obtiene el servicio HTTP y carga la información del usuario actual.
        /// </summary>
        public ProfileViewModel()
        {
            _httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
            _ = CargarUsuario();
        }

        /// <summary>
        /// Carga la información del usuario actual y actualiza las propiedades del ViewModel.
        /// </summary>
        [RelayCommand]
        private async Task CargarUsuario()
        {
            var user = await _httpService.GetCurrentUserInfo();
            if (user != null)
            {
                UserName = user.UserName;
                NombreCompleto = $"{user.Name} {user.Father_LastName} {user.Mother_LastName}";
                EsAdmin = user.Rol == "Admin";
            }
        }

        /// <summary>
        /// Navega a la pantalla para editar el perfil del usuario.
        /// </summary>
        [RelayCommand]
        private async Task EditarPerfil()
        {
            await Shell.Current.Navigation.PushAsync(new Views.EditProfilePage());
        }

        // Esta sección está comentada para futuras funciones de configuración
        //[RelayCommand] 
        //private async Task Configuracion()
        //{
        //    await Shell.Current.Navigation.PushAsync(new Views.SettingsPage());
        //}

        /// <summary>
        /// Cierra la sesión actual del usuario, limpia el almacenamiento seguro y redirige al LoginPage.
        /// </summary>
        [RelayCommand]
        private async Task CerrarSesion()
        {
            bool confirm = await App.Current.MainPage.DisplayAlert("Cerrar Sesión", "¿Estás seguro de que quieres cerrar sesión?", "Sí", "No");
            if (!confirm) return;

            _httpService.ResetAuthorization();       // Limpia la sesión activa
            SecureStorage.RemoveAll();               // Borra datos almacenados

            var loginPage = App.Current.Handler.MauiContext.Services.GetRequiredService<Views.LoginPage>();
            Application.Current.MainPage = new NavigationPage(loginPage);
        }

        /// <summary>
        /// Navega a la pantalla para registrar un nuevo usuario (solo disponible para Admins).
        /// </summary>
        [RelayCommand]
        private async Task CrearUsuario()
        {
            await Shell.Current.Navigation.PushAsync(new Views.CreateUserPage());
        }
    }
}
