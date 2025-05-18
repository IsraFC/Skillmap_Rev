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
        private void LoadSemesters()
        {
            allSemesters = new List<SemesterItem>
            {
                new SemesterItem
                {
                    Name = "Otoño 2022",
                    Subjects = new List<SubjectItem>
                    {
                        new SubjectItem { Name = "Visión global de la carrera", Semester = "Otoño 2022" },
                        new SubjectItem { Name = "Programación básica", Semester = "Otoño 2022" },
                        new SubjectItem { Name = "Estructura de datos", Semester = "Otoño 2022" }
                    }
                },
                new SemesterItem
                {
                    Name = "Primavera 2023",
                    Subjects = new List<SubjectItem>
                    {
                        new SubjectItem { Name = "Programación visual y videojuegos", Semester = "Primavera 2023" },
                        new SubjectItem { Name = "Sistemas operativos", Semester = "Primavera 2023" },
                        new SubjectItem { Name = "Bases de datos", Semester = "Primavera 2023" }
                    }
                },
                new SemesterItem
                {
                    Name = "Otoño 2023",
                    Subjects = new List<SubjectItem>
                    {
                        new SubjectItem { Name = "Pensamiento creativo", Semester = "Otoño 2023" },
                        new SubjectItem { Name = "Plataformas Abiertas I", Semester = "Otoño 2023" },
                        new SubjectItem { Name = "Ingeniería de software", Semester = "Otoño 2023" }
                    }
                },
                new SemesterItem
                {
                    Name = "Primavera 2024",
                    Subjects = new List<SubjectItem>
                    {
                        new SubjectItem { Name = "Redes de computadoras", Semester = "Primavera 2024" },
                        new SubjectItem { Name = "Computación en la nube", Semester = "Primavera 2024" },
                        new SubjectItem { Name = "Ciberseguridad", Semester = "Primavera 2024" }
                    }
                }
            };

            // Solo mostrar los últimos tres semestres en la pantalla principal
            var latestSemesters = allSemesters.TakeLast(3).ToList();
            semestersCollectionView.ItemsSource = latestSemesters;
        }

        /// <summary>
        /// Abre la pagina de la lista completa de semestres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnShowAllSemestersClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AllSemestersPage(allSemesters));
        }

        private async void OnSemesterSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count == 0)
                return;

            var selectedSemester = (SemesterItem)e.CurrentSelection.FirstOrDefault();
            await Navigation.PushAsync(new SemesterSubjectsPage(selectedSemester));

            // Limpiar la selección
            ((CollectionView)sender).SelectedItem = null;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

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
    }
}
