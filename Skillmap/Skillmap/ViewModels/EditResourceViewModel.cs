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
    /// ViewModel encargado de editar o eliminar recursos educativos.
    /// Permite modificar el contenido y la asociación de un recurso a una materia.
    /// </summary>
    public partial class EditResourceViewModel : ObservableObject
    {
        private readonly HttpService _httpService;

        /// <summary>
        /// Lista de todos los recursos disponibles.
        /// </summary>
        [ObservableProperty] private ObservableCollection<ResourcesItem> recursos;

        /// <summary>
        /// Lista de tipos de recurso disponibles.
        /// </summary>
        [ObservableProperty] private ObservableCollection<ResourceType> tipos;

        /// <summary>
        /// Lista de materias disponibles para vincular recursos.
        /// </summary>
        [ObservableProperty] private ObservableCollection<SubjectOutputDTO> materias;

        /// <summary>
        /// Recurso actualmente seleccionado para editar.
        /// </summary>
        [ObservableProperty] private ResourcesItem recursoSeleccionado;

        /// <summary>
        /// Tipo de recurso seleccionado (PDF, video, enlace, etc.).
        /// </summary>
        [ObservableProperty] private ResourceType tipoSeleccionado;

        /// <summary>
        /// Materia a la que se vinculará el recurso.
        /// </summary>
        [ObservableProperty] private SubjectOutputDTO materiaSeleccionada;

        [ObservableProperty] private string titulo;
        [ObservableProperty] private string descripcion;
        [ObservableProperty] private string link;
        [ObservableProperty] private bool esPublico = true;

        [ObservableProperty] private ImageSource imagenParaMostrar;
        [ObservableProperty] private string? textoBase64Imagen;

        private List<SubjectResource> subjectResources = new();

        /// <summary>
        /// Constructor que recibe el servicio HTTP y carga los datos necesarios.
        /// </summary>
        /// <param name="httpService">Instancia del servicio HTTP inyectado.</param>
        public EditResourceViewModel(HttpService httpService)
        {
            _httpService = httpService;
            CargarDatos();
        }

        /// <summary>
        /// Carga los recursos, tipos, materias y relaciones entre recursos y materias desde la API.
        /// </summary>
        private async void CargarDatos()
        {
            Recursos = new(await _httpService.GetResources());
            Tipos = new(await _httpService.GetResourceTypes());
            Materias = new(await _httpService.GetSubjects());
            subjectResources = await _httpService.GetAllSubjectResources();
        }

        /// <summary>
        /// Evento que se dispara al cambiar el recurso seleccionado.
        /// Rellena los campos de edición con la información correspondiente.
        /// </summary>
        /// <param name="value">Nuevo recurso seleccionado.</param>
        partial void OnRecursoSeleccionadoChanged(ResourcesItem value)
        {
            if (value == null) return;

            Titulo = value.Title;
            Descripcion = value.Description;
            Link = value.Link;
            TextoBase64Imagen = value.CoverImage;

            CargarImagenDesdeBase64(value.CoverImage);

            TipoSeleccionado = Tipos.FirstOrDefault(t => t.Id_Resource_Type == value.ResourceTypeId);

            var relacion = subjectResources.FirstOrDefault(sr => sr.ID_Resource == value.Id);
            MateriaSeleccionada = relacion != null
                ? Materias.FirstOrDefault(m => m.Id_Subject == relacion.ID_Subject)
                : null;
        }

        /// <summary>
        /// Comando para cambiar la imagen del recurso.
        /// </summary>
        [RelayCommand]
        public async Task CambiarImagen()
        {
            try
            {
                var result = await MediaPicker.Default.PickPhotoAsync();
                if (result != null)
                {
                    // 1. Mostrar la nueva imagen en pantalla
                    ImagenParaMostrar = ImageSource.FromFile(result.FullPath);

                    // 2. Convertir a Base64 para guardarla
                    byte[] imageArray = await File.ReadAllBytesAsync(result.FullPath);
                    TextoBase64Imagen = Convert.ToBase64String(imageArray);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo cargar la imagen: " + ex.Message, "OK");
            }
        }

        /// <summary>
        /// Comando que actualiza un recurso educativo y su relación con la materia.
        /// </summary>
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
                ResourceTypeId = TipoSeleccionado?.Id_Resource_Type ?? "",
                CoverImage = TextoBase64Imagen
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

        /// <summary>
        /// Comando que elimina un recurso seleccionado y su relación con la materia.
        /// Solicita confirmación antes de realizar la operación.
        /// </summary>
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

        // Método auxiliar para mostrar la imagen desde texto
        private void CargarImagenDesdeBase64(string base64)
        {
            if (string.IsNullOrEmpty(base64))
            {
                ImagenParaMostrar = null;
                return;
            }
            try
            {
                byte[] bytes = Convert.FromBase64String(base64);
                ImagenParaMostrar = ImageSource.FromStream(() => new MemoryStream(bytes));
            }
            catch
            {
                ImagenParaMostrar = null;
            }
        }
    }
}
