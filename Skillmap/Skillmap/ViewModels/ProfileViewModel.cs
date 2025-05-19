using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skillmap.Services;
using SkillmapLib1.Models.DTO.OutputDTO;

namespace Skillmap.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {
        [ObservableProperty] private string userName = string.Empty;
        [ObservableProperty] private string nombreCompleto = string.Empty;
        [ObservableProperty] private bool esAdmin;
        private readonly HttpService _httpService;

        public ProfileViewModel()
        {
            _httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
            _ = CargarUsuario();
        }

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

        [RelayCommand]
        private async Task EditarPerfil()
        {
            await Shell.Current.Navigation.PushAsync(new Views.EditProfilePage());
        }

        //[RelayCommand] 
        //private async Task Configuracion()
        //{
        //    await Shell.Current.Navigation.PushAsync(new Views.SettingsPage());
        //}

        [RelayCommand]
        private async Task CerrarSesion()
        {
            bool confirm = await App.Current.MainPage.DisplayAlert("Cerrar Sesión", "¿Estás seguro de que quieres cerrar sesión?", "Sí", "No");
            if (!confirm) return;

            // Cierra la sesión
            _httpService.ResetAuthorization();
            // Limpia SecureStorage
            SecureStorage.RemoveAll();

            // Redirige al LoginPage
            var loginPage = App.Current.Handler.MauiContext.Services.GetRequiredService<Views.LoginPage>();
            Application.Current.MainPage = new NavigationPage(loginPage);
        }

        [RelayCommand]
        private async Task CrearUsuario()
        {
            await Shell.Current.Navigation.PushAsync(new Views.CreateUserPage());
        }
    }
}
