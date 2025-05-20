using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkillmapLib1.Models;
using SkillmapLib1.Models.DTO.InputDTO;
using SkillmapLib1.Models.DTO.OutputDTO;
using Skillmap.Services;
using System.Collections.ObjectModel;

namespace Skillmap.ViewModels
{
    /// <summary>
    /// ViewModel responsable de la lógica para agregar un nuevo recurso educativo.
    /// Gestiona la entrada del usuario, validación y comunicación con la API.
    /// </summary>
    public partial class AddResourceViewModel : ObservableObject
    {
        private readonly HttpService _httpService;

        /// <summary>
        /// Lista de tipos de recurso disponibles para seleccionar.
        /// </summary>
        public ObservableCollection<ResourceType> Tipos { get; } = new();

        /// <summary>
        /// Lista de materias disponibles para asociar el recurso.
        /// </summary>
        public ObservableCollection<SubjectOutputDTO> Materias { get; } = new();

        // Propiedades enlazadas al formulario de la vista (por medio del toolkit MVVM)
        [ObservableProperty] private string titulo;
        [ObservableProperty] private string descripcion;
        [ObservableProperty] private string link;
        [ObservableProperty] private ResourceType tipoSeleccionado;
        [ObservableProperty] private SubjectOutputDTO materiaSeleccionada;
        [ObservableProperty] private bool esPublico = true;

        /// <summary>
        /// Constructor que recibe el servicio HTTP para interactuar con la API.
        /// </summary>
        /// <param name="httpService">Instancia del servicio HTTP inyectado.</param>
        public AddResourceViewModel(HttpService httpService)
        {
            _httpService = httpService;
        }

        /// <summary>
        /// Carga los tipos de recursos y materias disponibles desde la API.
        /// Llama a los endpoints correspondientes y actualiza las listas observables.
        /// </summary>
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

        /// <summary>
        /// Guarda un nuevo recurso educativo en la base de datos.
        /// Incluye validación de campos, creación del recurso y asociación con una materia.
        /// </summary>
        /// <returns>Una tarea asincrónica que representa el proceso completo de guardado.</returns>
        [RelayCommand]
        public async Task GuardarRecurso()
        {
            // Validación: todos los campos deben estar llenos
            if (string.IsNullOrWhiteSpace(Titulo) ||
                string.IsNullOrWhiteSpace(Descripcion) ||
                string.IsNullOrWhiteSpace(Link) ||
                TipoSeleccionado == null ||
                MateriaSeleccionada == null)
            {
                await Shell.Current.DisplayAlert("Campos incompletos", "Por favor llena todos los campos obligatorios.", "OK");
                return;
            }

            // Se crea el objeto del recurso con los datos proporcionados
            var nuevo = new ResourcesItem
            {
                Title = Titulo,
                Description = Descripcion,
                Link = Link,
                UploadDate = DateTime.Now,
                ResourceTypeId = TipoSeleccionado.Id_Resource_Type
            };

            // Se envía a la API para crear el recurso
            var creado = await _httpService.CreateResource(nuevo);
            if (creado == null || creado.Id == 0)
            {
                await Shell.Current.DisplayAlert("Error", "El recurso no se pudo crear correctamente.", "OK");
                return;
            }

            // Se crea la relación del recurso con la materia seleccionada
            var relacion = new SubjectResourceInputDTO
            {
                ID_Subject = MateriaSeleccionada.Id_Subject,
                ID_Resource = creado.Id
            };

            // Se envía a la API la relación recurso-materia
            var resp = await _httpService.CreateSubjectResource(relacion);
            if (!resp.IsSuccessStatusCode)
            {
                await Shell.Current.DisplayAlert("Error", "El recurso se creó, pero no se pudo vincular a una materia", "OK");
                return;
            }

            // Confirmación de éxito y navegación hacia atrás
            await Shell.Current.DisplayAlert("Éxito", "Recurso guardado correctamente", "OK");
            await Shell.Current.GoToAsync("..");
        }
    }
}
