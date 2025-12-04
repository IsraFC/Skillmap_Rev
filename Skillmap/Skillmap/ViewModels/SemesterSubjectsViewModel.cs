using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skillmap.Services;
using SkillmapLib1.Models.DTO.OutputDTO;
using System.Collections.ObjectModel;

namespace Skillmap.ViewModels
{
    /// <summary>
    /// ViewModel encargado de mostrar las materias correspondientes a un semestre específico.
    /// Permite cargar y filtrar materias, así como navegar a los recursos de una materia seleccionada.
    /// </summary>
    public partial class SemesterSubjectsViewModel : ObservableObject
    {
        private readonly HttpService _httpService;

        /// <summary>
        /// Colección de materias filtradas que pertenecen al semestre seleccionado.
        /// </summary>
        [ObservableProperty] private ObservableCollection<SubjectOutputDTO> materias = new();

        /// <summary>
        /// Materia actualmente seleccionada por el usuario.
        /// </summary>
        [ObservableProperty] private SubjectOutputDTO? materiaSeleccionada;

        /// <summary>
        /// Nombre del semestre que se está mostrando.
        /// </summary>
        [ObservableProperty] public string nombreSemestre = string.Empty;

        /// <summary>
        /// Constructor que inicializa el servicio HTTP.
        /// </summary>
        public SemesterSubjectsViewModel()
        {
            _httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
        }

        /// <summary>
        /// Carga todas las materias desde la API y filtra aquellas que pertenecen al semestre actual.
        /// </summary>
        [RelayCommand]
        private async Task CargarMaterias()
        {
            try
            {
                var allSubjects = await _httpService.GetSubjects();

                // Normaliza el nombre del semestre para comparar correctamente
                var nombreLimpio = NombreSemestre.Replace("°", "").Replace(" Semestre", "").Trim();

                var materiasFiltradas = allSubjects
                    .Where(s => s.Semester.Replace("°", "").Replace(" Semestre", "").Trim() == nombreLimpio)
                    .ToList();

                Materias = new ObservableCollection<SubjectOutputDTO>(materiasFiltradas);
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"No se pudieron cargar las materias: {ex.Message}", "OK");
            }
        }

        /// <summary>
        /// Navega a la pantalla de recursos de la materia seleccionada.
        /// </summary>
        [RelayCommand]
        private async Task Materia()
        {
            if (MateriaSeleccionada != null)
            {
                await Shell.Current.Navigation.PushAsync(new Views.SubjectResourcesPage(MateriaSeleccionada));
            }
        }
    }
}
