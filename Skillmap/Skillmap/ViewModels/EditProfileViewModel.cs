using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skillmap.Services;
using SkillmapLib1.Models.DTO.OutputDTO;
using System.Collections.ObjectModel;

namespace Skillmap.ViewModels
{
    /// <summary>
    /// ViewModel que permite la edición, carga y eliminación de perfiles de usuario.
    /// Su comportamiento se adapta según si el usuario es administrador o no.
    /// </summary>
    public partial class EditProfileViewModel : ObservableObject
    {
        /// <summary>
        /// Lista de usuarios disponibles. Visible solo para administradores.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<UserWithRoleOutputDTO> usuariosDisponibles = new();

        /// <summary>
        /// Usuario actualmente seleccionado para editar.
        /// </summary>
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

        /// <summary>
        /// Lista de roles disponibles que pueden asignarse al usuario.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<string> rolesDisponibles = new() { "Admin", "Teacher", "Student" };

        /// <summary>
        /// Indica si el usuario actual tiene rol de administrador.
        /// </summary>
        [ObservableProperty]
        private bool esAdmin;

        /// <summary>
        /// Constructor que inicializa el ViewModel con el servicio HTTP.
        /// </summary>
        public EditProfileViewModel()
        {
            _httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
        }

        /// <summary>
        /// Guarda los cambios realizados al usuario seleccionado.
        /// También actualiza la sesión si el usuario modificado es el logueado.
        /// </summary>
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

        /// <summary>
        /// Carga los datos del usuario actual o todos los usuarios si el rol es administrador.
        /// </summary>
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

                if (lista is not null && lista.Any())
                {
                    UsuariosDisponibles.Clear();
                    foreach (var u in lista)
                        UsuariosDisponibles.Add(u);

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

        /// <summary>
        /// Método disparado automáticamente al cambiar el usuario seleccionado.
        /// Rellena las propiedades del formulario con los datos del usuario.
        /// </summary>
        /// <param name="value">Nuevo usuario seleccionado.</param>
        partial void OnUsuarioSeleccionadoChanged(UserWithRoleOutputDTO value)
        {
            AplicarDatosUsuario();
        }

        /// <summary>
        /// Asigna los datos del usuario seleccionado a las propiedades editables.
        /// </summary>
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

        /// <summary>
        /// Elimina el usuario seleccionado tras una confirmación.
        /// Si es el usuario actual, cierra la sesión.
        /// </summary>
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

                if (UsuarioSeleccionado.UserName == usuarioLogueado)
                {
                    var loginPage = App.Current.Handler.MauiContext.Services.GetRequiredService<Views.LoginPage>();
                    Application.Current.MainPage = new NavigationPage(loginPage);
                }
                else
                {
                    await CargarDatos();

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

        /// <summary>
        /// Actualiza propiedades relacionadas al cambiar el estado de <c>EsAdmin</c>.
        /// </summary>
        /// <param name="value">Nuevo valor de EsAdmin.</param>
        partial void OnEsAdminChanged(bool value)
        {
            OnPropertyChanged(nameof(rolesDisponibles));
        }
    }
}
