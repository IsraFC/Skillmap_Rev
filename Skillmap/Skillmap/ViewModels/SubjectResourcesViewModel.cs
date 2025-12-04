using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skillmap.Services;
using SkillmapLib1.Models;
using SkillmapLib1.Models.DTO.OutputDTO;
using System.Collections.ObjectModel;
using Skillmap.Views;

namespace Skillmap.ViewModels
{
    /// <summary>
    /// ViewModel encargado de mostrar los recursos educativos asociados a una materia específica.
    /// Incluye funcionalidades de búsqueda y navegación al detalle del recurso.
    /// </summary>
    public partial class SubjectResourcesViewModel : ObservableObject
    {
        private readonly HttpService _httpService;

        /// <summary>
        /// Lista de recursos visibles tras aplicar un filtro (si existe).
        /// </summary>
        [ObservableProperty] private ObservableCollection<ResourcePerSubjectOutputDTO> recursosFiltrados = new();

        /// <summary>
        /// Texto ingresado para filtrar los recursos por título.
        /// </summary>
        [ObservableProperty] private string textoBusqueda = string.Empty;

        /// <summary>
        /// Materia actualmente seleccionada, cuyos recursos se están visualizando.
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NombreDocente))]
        [NotifyPropertyChangedFor(nameof(NombreMateria))]
        private SubjectOutputDTO? materiaSeleccionada;

        /// <summary>
        /// Nombre de la materia seleccionada (solo lectura).
        /// </summary>
        public string NombreMateria => MateriaSeleccionada?.Name ?? "";

        /// <summary>
        /// Nombre completo del docente asignado a la materia.
        /// </summary>
        public string NombreDocente => $"Docente: {MateriaSeleccionada?.TeacherFullName}";

        /// <summary>
        /// Lista completa de recursos sin filtrar, cargada desde la API.
        /// </summary>
        private List<ResourcePerSubjectOutputDTO> todosLosRecursos = new();

        /// <summary>
        /// Constructor que obtiene el servicio HTTP de forma inyectada.
        /// </summary>
        public SubjectResourcesViewModel()
        {
            _httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
        }

        /// <summary>
        /// Carga los recursos educativos asociados a la materia seleccionada desde la API.
        /// Aplica el filtro si hay texto de búsqueda.
        /// </summary>
        [RelayCommand]
        private async Task CargarRecursos()
        {
            if (MateriaSeleccionada == null)
                return;

            try
            {
                var recursos = await _httpService.GetResourcesBySubject(MateriaSeleccionada.Id_Subject);
                todosLosRecursos = recursos;
                AplicarFiltro();
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"No se pudieron cargar los recursos: {ex.Message}", "OK");
            }
        }

        /// <summary>
        /// Se ejecuta automáticamente cuando cambia el texto de búsqueda.
        /// Aplica el filtro a la lista de recursos.
        /// </summary>
        /// <param name="value">Nuevo texto ingresado.</param>
        partial void OnTextoBusquedaChanged(string value)
        {
            AplicarFiltro();
        }

        /// <summary>
        /// Filtra la lista de recursos según el texto de búsqueda actual.
        /// </summary>
        private void AplicarFiltro()
        {
            if (string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                RecursosFiltrados = new ObservableCollection<ResourcePerSubjectOutputDTO>(todosLosRecursos);
            }
            else
            {
                var texto = TextoBusqueda.ToLower();
                RecursosFiltrados = new ObservableCollection<ResourcePerSubjectOutputDTO>(
                    todosLosRecursos.Where(r => r.ResourceTitle.ToLower().Contains(texto)).ToList());
            }
        }

        /// <summary>
        /// Navega a la pantalla de detalle del recurso seleccionado.
        /// </summary>
        /// <param name="recurso">Recurso seleccionado.</param>
        [RelayCommand]
        private async Task VerMas(ResourcePerSubjectOutputDTO recurso)
        {
            await Shell.Current.Navigation.PushAsync(new ResourcesDetailPage(new ResourcesItem
            {
                Title = recurso.ResourceTitle,
                Description = recurso.Description,
                Link = recurso.Link,
                UploadDate = recurso.UploadDate
            }));
        }
    }
}
