using System.Threading.Tasks;
using Microsoft.Maui.Controls; // Importa los controles de .NET MAUI
using Skillmap.Models;
using Skillmap.Services;
using SkillmapLib1.Models;
using SkillmapLib1.Models.DTO.OutputDTO;

namespace Skillmap.Views // Define el espacio de nombres donde se encuentra la pantalla
{
    public partial class MainPage : ContentPage // Declara la clase MainPage, que representa la pantalla principal
    {
        private readonly HttpService _httpService;
        private List<SemesterItem> allSemesters = new List<SemesterItem>();

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainPage(HttpService httpService)
        {
            InitializeComponent(); // Inicializa los componentes definidos en MainPage.xaml
            _httpService = httpService;

            LoadSemesters();
        }

        /// <summary>
        /// Carga los datos correspondientes a los semestres
        /// </summary>
        private async Task LoadSemesters()
        {
            try
            {
                // Asegura que el token esté cargado antes de hacer peticiones
                var success = await _httpService.RestoreSession();
                if (!success)
                {
                    await DisplayAlert("Error", "Sesión no iniciada. Vuelve a iniciar sesión.", "OK");
                    return;
                }

                var subjects = await _httpService.GetSubjects();

                // Agrupar materias por semestre y convertirlas a SemesterItem
                var grouped = subjects
                    .GroupBy(s => s.Semester)
                    .Select(g => new SemesterItem
                    {
                        Name = g.Key,
                        Subjects = g.ToList()
                    })
                    .ToList();

                // guardar en el campo allSemesters
                allSemesters = grouped;

                semestersCollectionView.ItemsSource = grouped
                    .OrderByDescending(s => s.Name)
                    .Take(3)
                    .ToList();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudieron cargar los semestres: {ex.Message}", "OK");
            }
        }

        /// <summary>
        /// Abre la pagina de la lista completa de semestres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnShowAllSemestersClicked(object sender, EventArgs e)
        {
            // Pasa la lista de semestres al constructor de la página
            if (allSemesters is not null && allSemesters.Any())
            {
                await Navigation.PushAsync(new AllSemestersPage(allSemesters));
            }
            else
            {
                await DisplayAlert("Ups", "No hay semestres para mostrar.", "OK");
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            //  Recargar lista de semestres
            await LoadSemesters();

            // Cargar recomendaciones
            try
            {
                var recursos = await _httpService.GetResources(); // O GetResources si tienes ese método
                var aleatorios = recursos.OrderBy(x => Guid.NewGuid()).Take(2).ToList();
                recommendedCollection.ItemsSource = aleatorios;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudieron cargar los recursos: {ex.Message}", "OK");
            }


        }

        private void OnSemesterTapped(object sender, TappedEventArgs e)
        {
            if (sender is VisualElement view && view.BindingContext is SemesterItem tappedSemester)
            {
                Navigation.PushAsync(new SemesterSubjectsPage(tappedSemester));
            }
        }
    }
}
