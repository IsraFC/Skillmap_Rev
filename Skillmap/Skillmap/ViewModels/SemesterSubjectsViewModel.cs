using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skillmap.Services;
using SkillmapLib1.Models.DTO.OutputDTO;
using System.Collections.ObjectModel;

namespace Skillmap.ViewModels
{
    public partial class SemesterSubjectsViewModel : ObservableObject
    {
        private readonly HttpService _httpService;

        [ObservableProperty] private ObservableCollection<SubjectOutputDTO> materias = new();
        [ObservableProperty] private SubjectOutputDTO? materiaSeleccionada;
        [ObservableProperty] public string nombreSemestre = string.Empty;

        public SemesterSubjectsViewModel()
        {
            _httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
        }

        [RelayCommand]
        private async Task CargarMaterias()
        {
            try
            {
                var allSubjects = await _httpService.GetSubjects();

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
