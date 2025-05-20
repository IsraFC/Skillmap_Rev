using System.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skillmap.Services;
using SkillmapLib1.Models.DTO.InputDTO;

namespace Skillmap.ViewModels
{
    /// <summary>
    /// ViewModel responsable de la lógica para crear un nuevo usuario desde la interfaz.
    /// Permite registrar usuarios con diferentes roles dentro del sistema (Admin, Docente o Estudiante).
    /// </summary>
    public partial class CreateUserViewModel : ObservableObject
    {
        private readonly HttpService _httpService;

        /// <summary>
        /// Nombre de usuario (también se usará como correo electrónico).
        /// </summary>
        [ObservableProperty] private string userName = string.Empty;

        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        [ObservableProperty] private string name = string.Empty;

        /// <summary>
        /// Apellido paterno del usuario.
        /// </summary>
        [ObservableProperty] private string father_LastName = string.Empty;

        /// <summary>
        /// Apellido materno del usuario.
        /// </summary>
        [ObservableProperty] private string mother_LastName = string.Empty;

        /// <summary>
        /// Contraseña del usuario.
        /// </summary>
        [ObservableProperty] private string password = string.Empty;

        /// <summary>
        /// Rol asignado al usuario (Admin, Teacher o Student).
        /// </summary>
        [ObservableProperty] private string role = string.Empty;

        /// <summary>
        /// Lista de roles disponibles para seleccionar desde el formulario.
        /// </summary>
        public List<string> RolesDisponibles { get; } = new() { "Admin", "Teacher", "Student" };

        /// <summary>
        /// Constructor que inicializa el servicio HTTP y recupera la sesión actual si existe.
        /// </summary>
        public CreateUserViewModel()
        {
            _httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
            _ = _httpService.RestoreSession();
        }

        /// <summary>
        /// Comando que crea un nuevo usuario enviando los datos ingresados a la API.
        /// Realiza validación de campos obligatorios y muestra retroalimentación visual.
        /// </summary>
        [RelayCommand]
        private async Task CrearUsuario()
        {
            // Validación de campos obligatorios
            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Role))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Por favor llena todos los campos obligatorios.", "OK");
                return;
            }

            // Construcción del objeto de entrada para el nuevo usuario
            var nuevo = new UserInputDTO
            {
                UserName = UserName,
                Email = UserName, // En este sistema, el UserName también es usado como correo
                Name = Name,
                Father_LastName = Father_LastName,
                Mother_LastName = Mother_LastName,
                Password = Password,
                Rol = Role
            };

            // Envío del usuario a la API
            var response = await _httpService.PostAsJson("api/User", nuevo);
            var content = await response.Content.ReadAsStringAsync();

            // Validación de la respuesta de la API
            if (response.IsSuccessStatusCode)
            {
                await App.Current.MainPage.DisplayAlert("Éxito", "Usuario creado correctamente", "OK");
                await Shell.Current.Navigation.PopAsync(); // Regresar a la pantalla anterior
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", $"Error: {content}", "OK");
            }
        }
    }
}
