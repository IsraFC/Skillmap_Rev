using Skillmap.Services;
using SkillmapLib1.Models.DTO.InputDTO;
using SkillmapLib1.Models.DTO.OutputDTO;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Skillmap.ViewModels
{
    /// <summary>
    /// ViewModel que gestiona la edición y eliminación de materias en la plataforma SkillMap.
    /// Permite seleccionar una materia, modificar sus datos y actualizarla o eliminarla.
    /// </summary>
    public partial class EditSubjectViewModel : ObservableObject
    {
        private readonly HttpService _httpService;

        /// <summary>
        /// Colección de materias disponibles para editar.
        /// </summary>
        [ObservableProperty] private ObservableCollection<SubjectOutputDTO> materias = new();

        /// <summary>
        /// Lista de docentes disponibles para asignar a una materia.
        /// </summary>
        [ObservableProperty] private ObservableCollection<UserWithRoleOutputDTO> docentes = new();

        /// <summary>
        /// Lista de semestres disponibles.
        /// </summary>
        [ObservableProperty] private List<string> semestres = new() { "1° Semestre", "2° Semestre", "3° Semestre", "4° Semestre", "5° Semestre", "6° Semestre", "7° Semestre", "8° Semestre" };

        /// <summary>
        /// Materia actualmente seleccionada para editar.
        /// </summary>
        [ObservableProperty] private SubjectOutputDTO? materiaSeleccionada;

        /// <summary>
        /// Nombre de la materia en edición.
        /// </summary>
        [ObservableProperty] private string nombre = string.Empty;

        /// <summary>
        /// Semestre al que pertenece la materia.
        /// </summary>
        [ObservableProperty] private string semestreSeleccionado = string.Empty;

        /// <summary>
        /// Docente asignado a la materia.
        /// </summary>
        [ObservableProperty] private UserWithRoleOutputDTO? docenteSeleccionado;

        /// <summary>
        /// Indica si la materia está habilitada. Puede adaptarse a control de estados.
        /// </summary>
        [ObservableProperty] private bool habilitado = true;

        /// <summary>
        /// Determina si el formulario está habilitado para edición.
        /// </summary>
        [ObservableProperty] private bool puedeEditar = false;

        /// <summary>
        /// Comando para actualizar la materia seleccionada.
        /// </summary>
        public ICommand ActualizarCommand => new AsyncRelayCommand(ActualizarMateria);

        /// <summary>
        /// Comando para eliminar la materia seleccionada.
        /// </summary>
        public ICommand EliminarCommand => new AsyncRelayCommand(EliminarMateria);

        /// <summary>
        /// Constructor que inicializa el servicio y carga los datos necesarios.
        /// </summary>
        public EditSubjectViewModel()
        {
            _httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
            _ = CargarDatosAsync();
        }

        /// <summary>
        /// Evento que se dispara cuando cambia la materia seleccionada.
        /// Rellena los campos del formulario con los datos de la materia.
        /// </summary>
        /// <param name="value">Materia seleccionada.</param>
        partial void OnMateriaSeleccionadaChanged(SubjectOutputDTO? value)
        {
            PuedeEditar = value != null;

            if (value != null)
            {
                Nombre = value.Name;
                SemestreSeleccionado = value.Semester;
                DocenteSeleccionado = Docentes.FirstOrDefault(d => d.UserName == value.TeacherUserName);
                Habilitado = true; // Por si se implementa manejo de estado activo/inactivo
            }
        }

        /// <summary>
        /// Carga las materias y docentes desde la API para llenar los pickers.
        /// </summary>
        private async Task CargarDatosAsync()
        {
            var materiasList = await _httpService.GetSubjects();
            Materias = new ObservableCollection<SubjectOutputDTO>(materiasList);

            var usuarios = await _httpService.GetAllUsers();
            var soloDocentes = usuarios.Where(u => u.Rol == "Teacher").ToList();
            Docentes = new ObservableCollection<UserWithRoleOutputDTO>(soloDocentes);
        }

        /// <summary>
        /// Actualiza la materia con los datos ingresados por el usuario.
        /// </summary>
        private async Task ActualizarMateria()
        {
            if (MateriaSeleccionada == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Selecciona una materia primero.", "OK");
                return;
            }

            var actualizada = new SubjectInputDTO
            {
                Name = Nombre,
                Semester = SemestreSeleccionado,
                TeacherUserName = DocenteSeleccionado?.UserName ?? ""
            };

            var response = await _httpService.UpdateSubject(MateriaSeleccionada.Id_Subject, actualizada);

            if (!response.IsSuccessStatusCode)
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo actualizar la materia", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Éxito", "Materia actualizada correctamente", "OK");
            }
        }

        /// <summary>
        /// Elimina la materia seleccionada luego de una confirmación del usuario.
        /// </summary>
        private async Task EliminarMateria()
        {
            if (MateriaSeleccionada == null) return;

            bool confirm = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Estás seguro de eliminar esta materia?", "Sí", "No");
            if (!confirm) return;

            await _httpService.DeleteSubject(MateriaSeleccionada.Id_Subject);
            await App.Current.MainPage.DisplayAlert("Éxito", "Materia eliminada.", "OK");

            Materias.Remove(MateriaSeleccionada);
            MateriaSeleccionada = null;
            PuedeEditar = false;
        }
    }
}
