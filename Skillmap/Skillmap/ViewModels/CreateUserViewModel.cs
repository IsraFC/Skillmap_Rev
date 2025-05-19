using System.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skillmap.Services;
using SkillmapLib1.Models.DTO.InputDTO;

namespace Skillmap.ViewModels
{
    public partial class CreateUserViewModel : ObservableObject
    {
        private readonly HttpService _httpService;

        [ObservableProperty] private string userName = string.Empty;
        [ObservableProperty] private string name = string.Empty;
        [ObservableProperty] private string father_LastName = string.Empty;
        [ObservableProperty] private string mother_LastName = string.Empty;
        [ObservableProperty] private string password = string.Empty;
        [ObservableProperty] private string role = string.Empty;

        public List<string> RolesDisponibles { get; } = new() { "Admin", "Teacher", "Student" };

        public CreateUserViewModel()
        {
            _httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
            _ = _httpService.RestoreSession();
        }

        [RelayCommand]
        private async Task CrearUsuario()
        {
            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Role))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Por favor llena todos los campos obligatorios.", "OK");
                return;
            }

            var nuevo = new UserInputDTO
            {
                UserName = UserName,
                Email = UserName,
                Name = Name,
                Father_LastName = Father_LastName,
                Mother_LastName = Mother_LastName,
                Password = Password,
                Rol = Role
            };

            var response = await _httpService.PostAsJson("api/User", nuevo);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                await App.Current.MainPage.DisplayAlert("Éxito", "Usuario creado correctamente", "OK");
                await Shell.Current.Navigation.PopAsync();
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", $"Error: {content}", "OK");
            }
        }
    }
}
