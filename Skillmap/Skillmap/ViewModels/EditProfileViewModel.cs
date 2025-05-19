using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skillmap.Services;
using SkillmapLib1.Models.DTO.OutputDTO;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace Skillmap.ViewModels
{
    public partial class EditProfileViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<UserWithRoleOutputDTO> usuariosDisponibles = new();

        [ObservableProperty]
        private UserWithRoleOutputDTO usuarioSeleccionado;

        private string usuarioLogueado = string.Empty;

        private readonly HttpService _httpService;

        private string oldUserName = string.Empty;

        [ObservableProperty] private string userName = string.Empty;
        [ObservableProperty] private string name = string.Empty;
        [ObservableProperty] private string father_LastName = string.Empty;
        [ObservableProperty] private string mother_LastName = string.Empty;
        [ObservableProperty] private string rol = string.Empty;

        [ObservableProperty] private ObservableCollection<string> rolesDisponibles = new() { "Admin", "Teacher", "Student" };

        [ObservableProperty] private bool esAdmin;

        public EditProfileViewModel()
        {
            _httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
        }

        [RelayCommand]
        private async Task GuardarCambios()
        {
            if (UsuarioSeleccionado == null) return;

            var actualizado = new UpdateUserDTO
            {
                OldUserName = oldUserName,
                UserName = UserName,
                Email = UserName,
                Name = Name,
                Father_LastName = Father_LastName,
                Mother_LastName = Mother_LastName,
                Rol = Rol
            };

            var response = await _httpService.UpdateUser(actualizado);

            if (response.IsSuccessStatusCode)
            {
                // Si el usuario logueado fue editado, actualizamos SecureStorage
                if (UsuarioSeleccionado.UserName == usuarioLogueado)
                {
                    await SecureStorage.SetAsync("userName", actualizado.UserName);
                    await SecureStorage.SetAsync("userRole", actualizado.Rol);
                }

                await App.Current.MainPage.DisplayAlert("Éxito", "Perfil actualizado correctamente", "OK");

                await CargarDatos();
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "No se pudo actualizar el perfil", "OK");
            }
        }

        [RelayCommand]
        public async Task CargarDatos()
        {
            usuarioLogueado = await SecureStorage.GetAsync("userName");
            var rolGuardado = await SecureStorage.GetAsync("userRole");
            EsAdmin = rolGuardado == "Admin";

            string userToRestore = UsuarioSeleccionado?.UserName ?? usuarioLogueado;

            if (EsAdmin)
            {
                var lista = await _httpService.GetAllUsers();

                // Asegura que no haya valores nulos antes de asignar
                if (lista is not null && lista.Any())
                {
                    UsuariosDisponibles.Clear();
                    foreach (var u in lista)
                        UsuariosDisponibles.Add(u);

                    // Si el usuario anterior aún existe en la nueva lista, lo seleccionamos
                    var user = UsuariosDisponibles.FirstOrDefault(u => u.UserName == userToRestore);

                    UsuarioSeleccionado = user ?? UsuariosDisponibles.FirstOrDefault();
                }
                else
                {
                    UsuariosDisponibles.Clear();
                    UsuarioSeleccionado = null;
                }
            }
            else
            {
                var user = await _httpService.GetCurrentUserInfo();
                if (user != null)
                {
                    UsuariosDisponibles.Clear();
                    UsuariosDisponibles.Add(user);
                    UsuarioSeleccionado = user;
                }
            }

            AplicarDatosUsuario();
        }

        partial void OnUsuarioSeleccionadoChanged(UserWithRoleOutputDTO value)
        {
            AplicarDatosUsuario();
        }

        private void AplicarDatosUsuario()
        {
            if (UsuarioSeleccionado == null) return;

            oldUserName = UsuarioSeleccionado.UserName;
            UserName = UsuarioSeleccionado.UserName;
            Name = UsuarioSeleccionado.Name;
            Father_LastName = UsuarioSeleccionado.Father_LastName;
            Mother_LastName = UsuarioSeleccionado.Mother_LastName;
            Rol = UsuarioSeleccionado.Rol;
        }

        [RelayCommand]
        private async Task EliminarUsuario()
        {
            if (UsuarioSeleccionado == null) return;

            bool confirm = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Seguro que deseas eliminar este usuario?", "Sí", "No");
            if (!confirm) return;

            var response = await _httpService.DeleteUser(UsuarioSeleccionado.UserName);
            if (response.IsSuccessStatusCode)
            {
                await App.Current.MainPage.DisplayAlert("Éxito", "Usuario eliminado", "OK");

                // Si se eliminó el usuario logueado, cerrar sesión
                if (UsuarioSeleccionado.UserName == usuarioLogueado)
                {
                    var loginPage = App.Current.Handler.MauiContext.Services.GetRequiredService<Views.LoginPage>();
                    Application.Current.MainPage = new NavigationPage(loginPage);
                }
                else
                {
                    await CargarDatos(); // Recargar lista

                    //limpiar la selección si ya no existe
                    // Validación para evitar excepción
                    if (!UsuariosDisponibles.Any(u => u.UserName == UsuarioSeleccionado?.UserName))
                    {
                        UsuarioSeleccionado = UsuariosDisponibles.FirstOrDefault();
                    }
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "No se pudo eliminar el usuario", "OK");
            }
        }

        partial void OnEsAdminChanged(bool value)
        {
            OnPropertyChanged(nameof(rolesDisponibles));
        }
    }
}
