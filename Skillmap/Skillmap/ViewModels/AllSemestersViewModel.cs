using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkillmapLib1.Models;
using Skillmap.Services;
using System.Collections.ObjectModel;

namespace Skillmap.ViewModels
{
    /// <summary>
    /// ViewModel encargado de obtener, agrupar y mostrar las materias por semestre.
    /// También determina si el usuario tiene permisos administrativos o docentes.
    /// </summary>
    public partial class AllSemestersViewModel : ObservableObject
    {
        private readonly HttpService _httpService;

        /// <summary>
        /// Lista observable que contiene los semestres agrupados con sus materias respectivas.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<SemesterItem> semestres = new();

        /// <summary>
        /// Indica si el usuario actual tiene un rol diferente al de estudiante.
        /// Permite mostrar o esconder funciones según su rol.
        /// </summary>
        [ObservableProperty]
        private bool isNotStudent;

        /// <summary>
        /// Constructor que inicializa el ViewModel con el servicio HTTP.
        /// Llama a los métodos para cargar los semestres y verificar el rol del usuario.
        /// </summary>
        /// <param name="httpService">Instancia del servicio HTTP inyectado.</param>
        public AllSemestersViewModel(HttpService httpService)
        {
            _httpService = httpService;
            CargarSemestres();
            VerificarRol();
        }

        /// <summary>
        /// Verifica el rol del usuario almacenado en SecureStorage.
        /// Establece la propiedad <c>IsNotStudent</c> en true si el usuario es Admin o Teacher.
        /// </summary>
        private async void VerificarRol()
        {
            var rol = await SecureStorage.GetAsync("userRole");
            IsNotStudent = rol == "Admin" || rol == "Teacher";
        }

        /// <summary>
        /// Carga las materias desde la API, las agrupa por semestre
        /// y las transforma en una colección observable para su visualización.
        /// </summary>
        [RelayCommand]
        public async void CargarSemestres()
        {
            try
            {
                var subjects = await _httpService.GetSubjects();

                // Agrupar materias por semestre, ordenadas numéricamente
                var grouped = subjects
                    .OrderBy(s => int.Parse(s.Semester.Split('°')[0]))
                    .GroupBy(s => s.Semester)
                    .Select(g => new SemesterItem
                    {
                        Name = g.Key,
                        Subjects = g.ToList()
                    });

                // Asignar la colección agrupada a la propiedad observable
                Semestres = [.. grouped];
            }
            catch (Exception)
            {
                // Manejo de errores: se puede mostrar un mensaje o registrar el error
            }
        }
    }
}
