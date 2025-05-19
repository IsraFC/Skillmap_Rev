using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skillmap.Services;
using SkillmapLib1.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Skillmap.ViewModels
{
    public partial class ResourcesViewModel : ObservableObject
    {
        private readonly HttpService _httpService;

        [ObservableProperty]
        private bool isNotStudent;

        [ObservableProperty]
        private ObservableCollection<ResourcesItem> recursosFiltrados = new();

        private List<ResourcesItem> todosLosRecursos = new();

        [ObservableProperty]
        private string textoBusqueda = string.Empty;

        public ICommand VerMasCommand => new AsyncRelayCommand<ResourcesItem>(VerDetalle);
        public ICommand AgregarRecursoCommand => new AsyncRelayCommand(NavegarAgregar);
        public ICommand EditarRecursosCommand => new AsyncRelayCommand(NavegarEditar);

        public ResourcesViewModel()
        {
            _httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
            _ = CargarRecursos();
            VerificarRol();
        }

        private async void VerificarRol()
        {
            var rol = await SecureStorage.GetAsync("userRole");
            IsNotStudent = rol == "Admin" || rol == "Teacher";
        }

        partial void OnTextoBusquedaChanged(string value)
        {
            AplicarFiltro();
        }

        [RelayCommand]
        private async Task CargarRecursos()
        {
            try
            {
                todosLosRecursos = await _httpService.GetResources();
                AplicarFiltro();
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"No se pudieron cargar los recursos: {ex.Message}", "OK");
            }
        }

        private void AplicarFiltro()
        {
            if (string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                RecursosFiltrados = new ObservableCollection<ResourcesItem>(todosLosRecursos);
            }
            else
            {
                var texto = TextoBusqueda.ToLower();
                RecursosFiltrados = new ObservableCollection<ResourcesItem>(
                    todosLosRecursos.Where(r => r.Title.ToLower().Contains(texto)).ToList());
            }
        }

        private async Task VerDetalle(ResourcesItem recurso)
        {
            var detalle = new ResourcesItem
            {
                Title = recurso.Title,
                Description = recurso.Description,
                Link = recurso.Link,
                UploadDate = recurso.UploadDate
            };

            await Shell.Current.Navigation.PushAsync(new Views.ResourcesDetailPage(detalle));
        }

        private async Task NavegarAgregar()
        {
            await Shell.Current.Navigation.PushAsync(new Views.AddResourcePage());
        }

        private async Task NavegarEditar()
        {
            await Shell.Current.Navigation.PushAsync(new Views.EditResourcePage());
        }
    }
}
