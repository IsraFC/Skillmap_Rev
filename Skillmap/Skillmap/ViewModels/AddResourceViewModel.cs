using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkillmapLib1.Models;
using SkillmapLib1.Models.DTO.InputDTO;
using SkillmapLib1.Models.DTO.OutputDTO;
using Skillmap.Services;
using System.Collections.ObjectModel;

namespace Skillmap.ViewModels
{
    public partial class AddResourceViewModel : ObservableObject
    {
        private readonly HttpService _httpService;

        public ObservableCollection<ResourceType> Tipos { get; } = new();
        public ObservableCollection<SubjectOutputDTO> Materias { get; } = new();

        [ObservableProperty] private string titulo;
        [ObservableProperty] private string descripcion;
        [ObservableProperty] private string link;
        [ObservableProperty] private ResourceType tipoSeleccionado;
        [ObservableProperty] private SubjectOutputDTO materiaSeleccionada;
        [ObservableProperty] private bool esPublico = true;

        public AddResourceViewModel(HttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task CargarDatosAsync()
        {
            var tipos = await _httpService.GetResourceTypes();
            var materias = await _httpService.GetSubjects();

            Tipos.Clear();
            foreach (var tipo in tipos)
                Tipos.Add(tipo);

            Materias.Clear();
            foreach (var mat in materias)
                Materias.Add(mat);
        }

        [RelayCommand]
        public async Task GuardarRecurso()
        {
            if (string.IsNullOrWhiteSpace(Titulo) ||
                string.IsNullOrWhiteSpace(Descripcion) ||
                string.IsNullOrWhiteSpace(Link) ||
                TipoSeleccionado == null ||
                MateriaSeleccionada == null)
            {
                await Shell.Current.DisplayAlert("Campos incompletos", "Por favor llena todos los campos obligatorios.", "OK");
                return;
            }

            var nuevo = new ResourcesItem
            {
                Title = Titulo,
                Description = Descripcion,
                Link = Link,
                UploadDate = DateTime.Now,
                ResourceTypeId = TipoSeleccionado.Id_Resource_Type
            };

            var creado = await _httpService.CreateResource(nuevo);
            if (creado == null || creado.Id == 0)
            {
                await Shell.Current.DisplayAlert("Error", "El recurso no se pudo crear correctamente.", "OK");
                return;
            }

            var relacion = new SubjectResourceInputDTO
            {
                ID_Subject = MateriaSeleccionada.Id_Subject,
                ID_Resource = creado.Id
            };

            var resp = await _httpService.CreateSubjectResource(relacion);
            if (!resp.IsSuccessStatusCode)
            {
                await Shell.Current.DisplayAlert("Error", "El recurso se creó, pero no se pudo vincular a una materia", "OK");
                return;
            }

            await Shell.Current.DisplayAlert("Éxito", "Recurso guardado correctamente", "OK");
            await Shell.Current.GoToAsync(".."); // Retroceder
        }
    }
}
