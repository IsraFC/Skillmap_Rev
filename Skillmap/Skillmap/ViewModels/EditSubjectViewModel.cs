using Skillmap.Services;
using SkillmapLib1.Models.DTO.InputDTO;
using SkillmapLib1.Models.DTO.OutputDTO;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Skillmap.ViewModels
{
    public partial class EditSubjectViewModel : ObservableObject
    {
        private readonly HttpService _httpService;

        [ObservableProperty] private ObservableCollection<SubjectOutputDTO> materias = new();
        [ObservableProperty] private ObservableCollection<UserWithRoleOutputDTO> docentes = new();
        [ObservableProperty] private List<string> semestres = new() { "1° Semestre", "2° Semestre", "3° Semestre", "4° Semestre", "5° Semestre", "6° Semestre", "7° Semestre", "8° Semestre" };

        [ObservableProperty] private SubjectOutputDTO? materiaSeleccionada;
        [ObservableProperty] private string nombre = string.Empty;
        [ObservableProperty] private string semestreSeleccionado = string.Empty;
        [ObservableProperty] private UserWithRoleOutputDTO? docenteSeleccionado;
        [ObservableProperty] private bool habilitado = true;
        [ObservableProperty] private bool puedeEditar = false;

        public ICommand ActualizarCommand => new AsyncRelayCommand(ActualizarMateria);
        public ICommand EliminarCommand => new AsyncRelayCommand(EliminarMateria);

        public EditSubjectViewModel()
        {
            _httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
            _ = CargarDatosAsync();
        }

        partial void OnMateriaSeleccionadaChanged(SubjectOutputDTO? value)
        {
            PuedeEditar = value != null;
            if (value != null)
            {
                Nombre = value.Name;
                SemestreSeleccionado = value.Semester;
                DocenteSeleccionado = Docentes.FirstOrDefault(d => d.UserName == value.TeacherUserName);
                Habilitado = true; // Adaptar si se usa estado
            }
        }

        private async Task CargarDatosAsync()
        {
            var materiasList = await _httpService.GetSubjects();
            Materias = new ObservableCollection<SubjectOutputDTO>(materiasList);

            var usuarios = await _httpService.GetAllUsers();
            var soloDocentes = usuarios.Where(u => u.Rol == "Teacher").ToList();
            Docentes = new ObservableCollection<UserWithRoleOutputDTO>(soloDocentes);
        }

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
                return;
            }
            else
            {
                await Shell.Current.DisplayAlert("Éxito", "Materia actualizada correctamente", "OK");
            }
        }

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
