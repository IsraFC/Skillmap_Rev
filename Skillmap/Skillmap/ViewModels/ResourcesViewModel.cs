using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skillmap.Services;
using SkillmapLib1.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Skillmap.ViewModels
{
    /// <summary>
    /// ViewModel encargado de la visualización, filtrado, navegación y gestión de recursos educativos.
    /// Soporta acciones diferenciadas según el rol del usuario (Admin, Teacher, Student).
    /// </summary>
    public partial class ResourcesViewModel : ObservableObject
    {
        private readonly HttpService _httpService;

        /// <summary>
        /// Indica si el usuario actual tiene un rol distinto a "Student", habilitando funciones adicionales.
        /// </summary>
        [ObservableProperty]
        private bool isNotStudent;

        /// <summary>
        /// Colección de recursos visibles tras aplicar filtro (si existe).
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<ResourcesItem> recursosFiltrados = new();

        /// <summary>
        /// Lista interna con todos los recursos obtenidos desde la API.
        /// </summary>
        private List<ResourcesItem> todosLosRecursos = new();

        /// <summary>
        /// Texto ingresado por el usuario para filtrar los recursos por título.
        /// </summary>
        [ObservableProperty]
        private string textoBusqueda = string.Empty;

        /// <summary>
        /// Comando que navega a la vista de detalle del recurso seleccionado.
        /// </summary>
        public ICommand VerMasCommand => new AsyncRelayCommand<ResourcesItem>(VerDetalle);

        /// <summary>
        /// Comando que navega a la vista para agregar un nuevo recurso.
        /// </summary>
        public ICommand AgregarRecursoCommand => new AsyncRelayCommand(NavegarAgregar);

        /// <summary>
        /// Comando que navega a la vista de edición de recursos.
        /// </summary>
        public ICommand EditarRecursosCommand => new AsyncRelayCommand(NavegarEditar);

        /// <summary>
        /// Constructor que inicializa el ViewModel, carga los recursos y verifica el rol del usuario.
        /// </summary>
        public ResourcesViewModel()
        {
            _httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
            _ = CargarRecursos();
            VerificarRol();
        }

        /// <summary>
        /// Verifica el rol actual desde el almacenamiento seguro para habilitar funciones según el tipo de usuario.
        /// </summary>
        private async void VerificarRol()
        {
            var rol = await SecureStorage.GetAsync("userRole");
            IsNotStudent = rol == "Admin" || rol == "Teacher";
        }

        /// <summary>
        /// Evento que se dispara al modificar el texto de búsqueda.
        /// Aplica automáticamente el filtro de recursos.
        /// </summary>
        /// <param name="value">Texto nuevo ingresado.</param>
        partial void OnTextoBusquedaChanged(string value)
        {
            AplicarFiltro();
        }

        /// <summary>
        /// Obtiene todos los recursos desde la API y aplica el filtro de búsqueda.
        /// </summary>
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

        /// <summary>
        /// Aplica el filtro de búsqueda por título a la lista de recursos.
        /// </summary>
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

        /// <summary>
        /// Navega a la vista de detalle del recurso seleccionado.
        /// </summary>
        /// <param name="recurso">Recurso a visualizar.</param>
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

        /// <summary>
        /// Navega a la vista para agregar un nuevo recurso educativo.
        /// </summary>
        private async Task NavegarAgregar()
        {
            await Shell.Current.Navigation.PushAsync(new Views.AddResourcePage());
        }

        /// <summary>
        /// Navega a la vista para editar recursos educativos existentes.
        /// </summary>
        private async Task NavegarEditar()
        {
            await Shell.Current.Navigation.PushAsync(new Views.EditResourcePage());
        }
    }
}
