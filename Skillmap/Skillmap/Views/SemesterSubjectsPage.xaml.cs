using System.Threading.Tasks;
using Skillmap.Models;
using Skillmap.Services;
using SkillmapLib1.Models.DTO.OutputDTO;

namespace Skillmap.Views;

public partial class SemesterSubjectsPage : ContentPage
{
    private readonly string _nombreSemestre;
    private readonly HttpService _httpService;

    /// <summary>
    /// Constructor por defecto de pagina de materias
    /// </summary>
    /// <param name="semester"></param>
	public SemesterSubjectsPage(SemesterItem semester)
	{
		InitializeComponent();
        _nombreSemestre = semester.Name;
        SemesterTitle.Text = _nombreSemestre;

        // Obtener el HttpService desde DI
        _httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            var allSubjects = await _httpService.GetSubjects();

            // Estandarizar nombre del semestre (quitar espacios y "°")
            var nombreLimpio = _nombreSemestre.Replace("°", "").Replace(" Semestre", "").Trim();

            var materiasFiltradas = allSubjects
                .Where(s => s.Semester.Trim().Replace("°", "").Replace(" Semestre", "") == nombreLimpio)
                .ToList();

            subjectsCollectionView.ItemsSource = materiasFiltradas;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudieron cargar las materias: {ex.Message}", "OK");
        }
    }

    private async void OnSubjectSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is SubjectOutputDTO selectedSubject)
        {
            await Navigation.PushAsync(new SubjectResourcesPage(selectedSubject));
        }

        // Opcional: deselecciona el ítem para evitar selecciones bloqueadas
        ((CollectionView)sender).SelectedItem = null;
    }
}