using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkillmapLib1.Models;
using SkillmapLib1.Models.DTO.InputDTO;
using SkillmapLib1.Models.DTO.OutputDTO;
using Skillmap.Services;
using System.Collections.ObjectModel;

namespace Skillmap.ViewModels
{
    public partial class EditResourceViewModel : ObservableObject
    {
        private readonly HttpService _httpService;

        [ObservableProperty] private ObservableCollection<ResourcesItem> recursos;
        [ObservableProperty] private ObservableCollection<ResourceType> tipos;
        [ObservableProperty] private ObservableCollection<SubjectOutputDTO> materias;

        [ObservableProperty] private ResourcesItem recursoSeleccionado;
        [ObservableProperty] private ResourceType tipoSeleccionado;
        [ObservableProperty] private SubjectOutputDTO materiaSeleccionada;

        [ObservableProperty] private string titulo;
        [ObservableProperty] private string descripcion;
        [ObservableProperty] private string link;
        [ObservableProperty] private bool esPublico = true;

        private List<SubjectResource> subjectResources = new();

        public EditResourceViewModel(HttpService httpService)
        {
            _httpService = httpService;
            CargarDatos();
        }

        private async void CargarDatos()
        {
            Recursos = new(await _httpService.GetResources());
            Tipos = new(await _httpService.GetResourceTypes());
            Materias = new(await _httpService.GetSubjects());
            subjectResources = await _httpService.GetAllSubjectResources();
        }

        partial void OnRecursoSeleccionadoChanged(ResourcesItem value)
        {
            if (value == null) return;
            Titulo = value.Title;
            Descripcion = value.Description;
            Link = value.Link;

            TipoSeleccionado = Tipos.FirstOrDefault(t => t.Id_Resource_Type == value.ResourceTypeId);

            var relacion = subjectResources.FirstOrDefault(sr => sr.ID_Resource == value.Id);
            MateriaSeleccionada = relacion != null
                ? Materias.FirstOrDefault(m => m.Id_Subject == relacion.ID_Subject)
            : null;
        }

        [RelayCommand]
        private async Task Actualizar()
        {
            if (RecursoSeleccionado == null) return;

            var actualizado = new ResourcesItem
            {
                Id = RecursoSeleccionado.Id,
                Title = Titulo,
                Description = Descripcion,
                Link = Link,
                UploadDate = DateTime.Now,
                ResourceTypeId = TipoSeleccionado?.Id_Resource_Type ?? ""
            };

            var res = await _httpService.UpdateResource(actualizado.Id, actualizado);

            if (res.IsSuccessStatusCode)
            {
                var relacionExistente = subjectResources.FirstOrDefault(r => r.ID_Resource == actualizado.Id);
                if (relacionExistente != null)
                    await _httpService.DeleteSubjectResource(relacionExistente.ID_Subject, relacionExistente.ID_Resource);

                await _httpService.CreateSubjectResource(new SubjectResourceInputDTO
                {
                    ID_Subject = MateriaSeleccionada.Id_Subject,
                    ID_Resource = actualizado.Id
                });

                await Shell.Current.DisplayAlert("✅", "Recurso actualizado correctamente", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo actualizar el recurso", "OK");
            }
        }

        [RelayCommand]
        private async Task Eliminar()
        {
            if (RecursoSeleccionado == null) return;

            var confirm = await Shell.Current.DisplayAlert("¿Estás seguro?", "Esto eliminará el recurso permanentemente.", "Sí", "Cancelar");
            if (!confirm) return;

            var relacion = subjectResources.FirstOrDefault(r => r.ID_Resource == RecursoSeleccionado.Id);
            if (relacion != null)
                await _httpService.DeleteSubjectResource(relacion.ID_Subject, relacion.ID_Resource);

            await _httpService.DeleteResource(RecursoSeleccionado.Id);
            await Shell.Current.DisplayAlert("Listo", "Recurso eliminado correctamente", "OK");
            await Shell.Current.GoToAsync("..");
        }
    }
}
