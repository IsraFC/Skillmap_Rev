using SkillmapLib1.Models.DTO.InputDTO;
using Skillmap.Services;
using SkillmapLib1.Models.DTO.OutputDTO;

namespace Skillmap.Views;

public partial class CrearMateriaPage : ContentPage
{
    private readonly HttpService _httpService;
    private List<UserWithRoleOutputDTO> _docentes = new();


    public CrearMateriaPage()
	{
		InitializeComponent();

        // Obtener el HttpService desde el contenedor de servicios (DI)
        _httpService = ((App)Application.Current).ServiceProvider.GetService<HttpService>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Cargar docentes si aún no se han cargado
        if (!_docentes.Any())
        {
            var usuarios = await _httpService.GetAllUsers();

            _docentes = usuarios
                .Where(u => u.Rol == "Teacher")
                .ToList();

            // Mostrar nombre completo en el Picker
            DocentePicker.ItemsSource = _docentes;
            DocentePicker.ItemDisplayBinding = new Binding("NombreCompleto");
        }
    }

    private async void OnRegistrarMateriaClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NombreEntry.Text) ||
            SemestrePicker.SelectedIndex == -1 ||
            DocentePicker.SelectedIndex == -1)
        {
            await DisplayAlert("Campos incompletos", "Por favor llena todos los campos obligatorios.", "OK");
            return;
        }

        var selectedDocente = DocentePicker.SelectedItem as UserWithRoleOutputDTO;

        var nuevaMateria = new SubjectInputDTO
        {
            Name = NombreEntry.Text,
            Semester = SemestrePicker.SelectedItem.ToString(),
            TeacherUserName = selectedDocente?.UserName ?? ""
        };

        var response = await _httpService.CreateSubject(nuevaMateria);

        if (response.IsSuccessStatusCode)
        {
            await DisplayAlert("Éxito", "Materia registrada correctamente.", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Error", $"No se pudo registrar la materia.\n{response.StatusCode}", "OK");
        }
    }
}