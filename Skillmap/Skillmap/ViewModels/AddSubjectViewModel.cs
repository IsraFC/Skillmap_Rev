using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkillmapLib1.Models.DTO.InputDTO;
using SkillmapLib1.Models.DTO.OutputDTO;
using Skillmap.Services;
using System.Collections.ObjectModel;

namespace Skillmap.ViewModels
{
    /// <summary>
    /// ViewModel encargado de la lógica para registrar una nueva materia.
    /// Proporciona propiedades y comandos vinculados al formulario de creación de materias.
    /// </summary>
    public partial class AddSubjectViewModel : ObservableObject
    {
        private readonly HttpService _httpService;

        /// <summary>
        /// Inicializa el ViewModel, configura el servicio HTTP y carga los docentes disponibles.
        /// </summary>
        public AddSubjectViewModel()
        {
            _httpService = ((App)Application.Current).ServiceProvider.GetService<HttpService>();

            Semestres = new ObservableCollection<string>
            {
                "1° Semestre", "2° Semestre", "3° Semestre", "4° Semestre",
                "5° Semestre", "6° Semestre", "7° Semestre", "8° Semestre"
            };

            Docentes = new ObservableCollection<UserWithRoleOutputDTO>();
            LoadDocentes(); // Carga inicial de los docentes
        }

        /// <summary>
        /// Nombre de la materia ingresada por el usuario.
        /// </summary>
        [ObservableProperty] private string nombreMateria;

        /// <summary>
        /// Semestre seleccionado al que se asignará la materia.
        /// </summary>
        [ObservableProperty] private string semestreSeleccionado;

        /// <summary>
        /// Docente seleccionado para impartir la materia.
        /// </summary>
        [ObservableProperty] private UserWithRoleOutputDTO docenteSeleccionado;

        /// <summary>
        /// Indica si la materia estará habilitada tras su registro.
        /// </summary>
        [ObservableProperty] private bool estaHabilitada = true;

        /// <summary>
        /// Lista de opciones de semestre disponibles.
        /// </summary>
        public ObservableCollection<string> Semestres { get; set; }

        /// <summary>
        /// Lista de docentes disponibles para asignar a la materia.
        /// </summary>
        public ObservableCollection<UserWithRoleOutputDTO> Docentes { get; set; }

        /// <summary>
        /// Carga los usuarios con rol "Teacher" desde la API y los agrega a la lista de docentes.
        /// </summary>
        private async void LoadDocentes()
        {
            var lista = await _httpService.GetAllUsers();
            var docentes = lista.Where(u => u.Rol == "Teacher").ToList();

            Docentes.Clear();
            foreach (var d in docentes)
                Docentes.Add(d);
        }

        /// <summary>
        /// Comando que registra una nueva materia con los datos proporcionados por el usuario.
        /// Incluye validación y retroalimentación visual.
        /// </summary>
        [RelayCommand]
        private async Task Registrar()
        {
            // Validación de campos obligatorios
            if (string.IsNullOrWhiteSpace(NombreMateria) ||
                string.IsNullOrWhiteSpace(SemestreSeleccionado) ||
                DocenteSeleccionado == null)
            {
                await Shell.Current.DisplayAlert("Campos incompletos", "Por favor llena todos los campos obligatorios.", "OK");
                return;
            }

            // Creación del DTO para registrar la nueva materia
            var nuevaMateria = new SubjectInputDTO
            {
                Name = NombreMateria,
                Semester = SemestreSeleccionado,
                TeacherUserName = DocenteSeleccionado.UserName
            };

            // Envío a la API
            var response = await _httpService.CreateSubject(nuevaMateria);

            // Validación de la respuesta de la API
            if (response.IsSuccessStatusCode)
            {
                await Shell.Current.DisplayAlert("Éxito", "Materia registrada correctamente.", "OK");
                await Shell.Current.Navigation.PopAsync(); // Regresa a la pantalla anterior
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudo registrar la materia.\n{response.StatusCode}", "OK");
            }
        }
    }
}
