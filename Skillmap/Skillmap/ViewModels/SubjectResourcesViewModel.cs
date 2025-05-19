using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skillmap.Services;
using SkillmapLib1.Models;
using SkillmapLib1.Models.DTO.OutputDTO;
using System.Collections.ObjectModel;
using Skillmap.Views;

namespace Skillmap.ViewModels
{
    public partial class SubjectResourcesViewModel : ObservableObject
    {
        private readonly HttpService _httpService;

        [ObservableProperty] private ObservableCollection<ResourcePerSubjectOutputDTO> recursosFiltrados = new();
        [ObservableProperty] private string textoBusqueda = string.Empty;

        [ObservableProperty] public SubjectOutputDTO? materiaSeleccionada;
        public string NombreMateria => MateriaSeleccionada?.Name ?? "";
        public string NombreDocente => $"Docente: {MateriaSeleccionada?.TeacherFullName}";

        private List<ResourcePerSubjectOutputDTO> todosLosRecursos = new();

        public SubjectResourcesViewModel()
        {
            _httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
        }

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

        partial void OnTextoBusquedaChanged(string value)
        {
            AplicarFiltro();
        }

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
