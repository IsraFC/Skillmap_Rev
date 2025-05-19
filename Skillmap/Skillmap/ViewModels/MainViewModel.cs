using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skillmap.Services;
using SkillmapLib1.Models;
using SkillmapLib1.Models.DTO.OutputDTO;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Skillmap.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly HttpService _httpService;

        [ObservableProperty]
        private ObservableCollection<SemesterItem> semestresVisibles = new();

        [ObservableProperty]
        private ObservableCollection<ResourcesItem> recomendaciones = new();

        private List<SemesterItem> todosLosSemestres = new();

        public ICommand MostrarTodosCommand => new AsyncRelayCommand(NavegarATodos);
        public ICommand SemestreTappedCommand => new AsyncRelayCommand<SemesterItem>(AbrirSemestre);

        public MainViewModel()
        {
            _httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
            _ = InicializarVista();
        }

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

                todosLosSemestres = subjects
                    .GroupBy(s => s.Semester)
                    .Select(g => new SemesterItem
                    {
                        Name = g.Key,
                        Subjects = g.ToList()
                    })
                    .ToList();

                SemestresVisibles = new ObservableCollection<SemesterItem>(
                    todosLosSemestres.OrderByDescending(s => s.Name).Take(3));

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

        private async Task AbrirSemestre(SemesterItem semestre)
        {
            await Shell.Current.Navigation.PushAsync(new Views.SemesterSubjectsPage(semestre));
        }
    }
}
