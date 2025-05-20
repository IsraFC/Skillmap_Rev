using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skillmap.Services;
using SkillmapLib1.Models;
using SkillmapLib1.Models.DTO.OutputDTO;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Skillmap.ViewModels
{
    /// <summary>
    /// ViewModel principal que carga la vista inicial del usuario.
    /// Muestra los últimos semestres y recursos recomendados.
    /// </summary>
    public partial class MainViewModel : ObservableObject
    {
        private readonly HttpService _httpService;

        /// <summary>
        /// Semestres que se mostrarán en la pantalla principal (últimos 3).
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<SemesterItem> semestresVisibles = new();

        /// <summary>
        /// Recursos educativos sugeridos al usuario como recomendaciones.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<ResourcesItem> recomendaciones = new();

        /// <summary>
        /// Lista completa de todos los semestres, usada internamente.
        /// </summary>
        private List<SemesterItem> todosLosSemestres = new();

        /// <summary>
        /// Comando para navegar a la página que muestra todos los semestres.
        /// </summary>
        public ICommand MostrarTodosCommand => new AsyncRelayCommand(NavegarATodos);

        /// <summary>
        /// Comando para abrir los detalles de un semestre específico al hacer clic.
        /// </summary>
        public ICommand SemestreTappedCommand => new AsyncRelayCommand<SemesterItem>(AbrirSemestre);

        /// <summary>
        /// Constructor que obtiene el servicio HTTP y lanza la inicialización de la vista.
        /// </summary>
        public MainViewModel()
        {
            _httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
            _ = InicializarVista();
        }

        /// <summary>
        /// Inicializa la vista principal: verifica la sesión,
        /// carga semestres y genera recomendaciones aleatorias.
        /// </summary>
        [RelayCommand]
        private async Task InicializarVista()
        {
            try
            {
                var success = await _httpService.RestoreSession();
                if (!success)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Sesión no iniciada. Vuelve a iniciar sesión.", "OK");
                    return;
                }

                var subjects = await _httpService.GetSubjects();

                // Agrupar materias por semestre
                todosLosSemestres = subjects
                    .GroupBy(s => s.Semester)
                    .Select(g => new SemesterItem
                    {
                        Name = g.Key,
                        Subjects = g.ToList()
                    })
                    .ToList();

                // Mostrar los últimos 3 semestres
                SemestresVisibles = new ObservableCollection<SemesterItem>(
                    todosLosSemestres.OrderByDescending(s => s.Name).Take(3));

                // Generar recomendaciones aleatorias
                var recursos = await _httpService.GetResources();
                var random = new Random();
                Recomendaciones = new ObservableCollection<ResourcesItem>(
                    recursos.OrderBy(x => random.Next()).Take(2).ToList());
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"Hubo un problema: {ex.Message}", "OK");
            }
        }

        /// <summary>
        /// Navega a la vista que muestra todos los semestres con sus materias.
        /// </summary>
        private async Task NavegarATodos()
        {
            if (todosLosSemestres.Any())
            {
                await Shell.Current.Navigation.PushAsync(new Views.AllSemestersPage(todosLosSemestres));
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Ups", "No hay semestres para mostrar.", "OK");
            }
        }

        /// <summary>
        /// Abre la página de materias correspondientes al semestre seleccionado.
        /// </summary>
        /// <param name="semestre">Semestre seleccionado desde la vista principal.</param>
        private async Task AbrirSemestre(SemesterItem semestre)
        {
            await Shell.Current.Navigation.PushAsync(new Views.SemesterSubjectsPage(semestre));
        }
    }
}
