using Skillmap.Services;
using SkillmapLib1.Models.DTO.InputDTO;
using SkillmapLib1.Models.DTO.OutputDTO;

namespace Skillmap.Views;

public partial class EditarMateriaPage : ContentPage
{
    private readonly HttpService _httpService;
    private List<SubjectOutputDTO> _materias = new();
    private List<UserWithRoleOutputDTO> _docentes = new();
    private SubjectOutputDTO? _materiaSeleccionada;

    public EditarMateriaPage()
	{
		InitializeComponent();
        _httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        HabilitarControles(false);

        // Cargar materias
        _materias = await _httpService.GetSubjects();
        MateriaPicker.ItemsSource = _materias;
        MateriaPicker.ItemDisplayBinding = new Binding("Name");

        // Cargar docentes
        var usuarios = await _httpService.GetAllUsers();
        _docentes = usuarios.Where(u => u.Rol == "Teacher").ToList();
        DocentePicker.ItemsSource = _docentes;
        DocentePicker.ItemDisplayBinding = new Binding("NombreCompleto");
    }

    private void OnMateriaSelected(object sender, EventArgs e)
    {
        if (MateriaPicker.SelectedItem is SubjectOutputDTO materia)
        {
            HabilitarControles(true);

            _materiaSeleccionada = materia;
            NombreEntry.Text = materia.Name;
            SemestrePicker.SelectedItem = materia.Semester;
            DocentePicker.SelectedItem = _docentes.FirstOrDefault(d => d.UserName == materia.TeacherUserName);
            //HabilitadoCheckBox.IsChecked = materia.IsActive;
        }
        else
            HabilitarControles(false);
    }

    private async void OnActualizarMateriaClicked(object sender, EventArgs e)
    {
        if (_materiaSeleccionada == null)
        {
            await DisplayAlert("Error", "Selecciona una materia primero.", "OK");
            return;
        }

        var actualizada = new SubjectInputDTO
        {
            Name = NombreEntry.Text,
            Semester = SemestrePicker.SelectedItem?.ToString() ?? "",
            TeacherUserName = (DocentePicker.SelectedItem as UserWithRoleOutputDTO)?.UserName ?? "",
            // No olvides agregar una propiedad IsActive en tu SubjectInputDTO si no existe
        };

        var response = await _httpService.UpdateSubject(_materiaSeleccionada.Id_Subject, actualizada);

        if (response.IsSuccessStatusCode)
        {
            await DisplayAlert("Éxito", "Materia actualizada correctamente.", "OK");
            LimpiarCampos();
        }
        else
        {
            await DisplayAlert("Error", "No se pudo actualizar la materia.", "OK");
            LimpiarCampos();
        }
    }

    private async void OnEliminarMateriaClicked(object sender, EventArgs e)
    {
        if (_materiaSeleccionada == null)
        {
            await DisplayAlert("Error", "Selecciona una materia primero.", "OK");
            return;
        }

        var confirm = await DisplayAlert("Confirmar", "¿Estás seguro de eliminar esta materia?", "Sí", "No");
        if (!confirm) return;

        await _httpService.DeleteSubject(_materiaSeleccionada.Id_Subject);
        await DisplayAlert("Éxito", "Materia eliminada.", "OK");
        LimpiarCampos();
        MateriaPicker.SelectedItem = null;
    }

    private void LimpiarCampos()
    {
        MateriaPicker.SelectedItem = null;
        NombreEntry.Text = string.Empty;
        SemestrePicker.SelectedIndex = -1;
        DocentePicker.SelectedIndex = -1;
        HabilitadoCheckBox.IsChecked = true;
        _materiaSeleccionada = null;
    }

    private void HabilitarControles(bool habilitar)
    {
        NombreEntry.IsEnabled = habilitar;
        SemestrePicker.IsEnabled = habilitar;
        DocentePicker.IsEnabled = habilitar;
        HabilitadoCheckBox.IsEnabled = habilitar;
        ActualizarButton.IsEnabled = habilitar;
        EliminarButton.IsEnabled = habilitar;
    }
}