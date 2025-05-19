using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkillmapLib1.Models.DTO.InputDTO;
using SkillmapLib1.Models.DTO.OutputDTO;
using Skillmap.Services;
using System.Collections.ObjectModel;

namespace Skillmap.ViewModels
{
    public partial class AddSubjectViewModel : ObservableObject
    {
        private readonly HttpService _httpService;

        public AddSubjectViewModel()
        {
            _httpService = ((App)Application.Current).ServiceProvider.GetService<HttpService>();
            Semestres = new ObservableCollection<string>
            {
                "1° Semestre", "2° Semestre", "3° Semestre", "4° Semestre",
                "5° Semestre", "6° Semestre", "7° Semestre", "8° Semestre"
            };
            Docentes = new ObservableCollection<UserWithRoleOutputDTO>();
            LoadDocentes();
        }

        [ObservableProperty]
        private string nombreMateria;

        [ObservableProperty]
        private string semestreSeleccionado;

        [ObservableProperty]
        private UserWithRoleOutputDTO docenteSeleccionado;

        [ObservableProperty]
        private bool estaHabilitada = true;

        public ObservableCollection<string> Semestres { get; set; }

        public ObservableCollection<UserWithRoleOutputDTO> Docentes { get; set; }

        private async void LoadDocentes()
        {
            var lista = await _httpService.GetAllUsers();
            var docentes = lista.Where(u => u.Rol == "Teacher").ToList();
            Docentes.Clear();
            foreach (var d in docentes)
                Docentes.Add(d);
        }

        [RelayCommand]
        private async Task Registrar()
        {
            if (string.IsNullOrWhiteSpace(NombreMateria) ||
                string.IsNullOrWhiteSpace(SemestreSeleccionado) ||
                DocenteSeleccionado == null)
            {
                await Shell.Current.DisplayAlert("Campos incompletos", "Por favor llena todos los campos obligatorios.", "OK");
                return;
            }

            var nuevaMateria = new SubjectInputDTO
            {
                Name = NombreMateria,
                Semester = SemestreSeleccionado,
                TeacherUserName = DocenteSeleccionado.UserName
            };

            var response = await _httpService.CreateSubject(nuevaMateria);

            if (response.IsSuccessStatusCode)
            {
                await Shell.Current.DisplayAlert("Éxito", "Materia registrada correctamente.", "OK");
                await Shell.Current.Navigation.PopAsync();
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudo registrar la materia.\n{response.StatusCode}", "OK");
            }
        }
    }
}
