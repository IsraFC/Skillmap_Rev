using SkillmapLib1.Models;
using Skillmap.Services;
using SkillmapLib1.Models.DTO.OutputDTO;
using SkillmapLib1.Models.DTO.InputDTO;
using System.Net.Http.Json;
using Microsoft.Maui.Controls.Shapes;

namespace Skillmap.Views
{
    public partial class CrearRecursoPage : ContentPage
    {
        private readonly HttpService _httpService;
        private List<ResourceType> _tipos = new();
        private List<SubjectOutputDTO> _materias = new();

        public CrearRecursoPage()
        {
            InitializeComponent();

            _httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _tipos = await _httpService.GetResourceTypes();
            _materias = await _httpService.GetSubjects();

            TipoRecursoPicker.ItemsSource = _tipos;
            TipoRecursoPicker.ItemDisplayBinding = new Binding("Id_Resource_Type");

            MateriaPicker.ItemsSource = _materias;
            MateriaPicker.ItemDisplayBinding = new Binding("Name");
        }

        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TituloEntry.Text) ||
            string.IsNullOrWhiteSpace(DescripcionEditor.Text) ||
            string.IsNullOrWhiteSpace(RutaArchivoEntry.Text) ||
            TipoRecursoPicker.SelectedItem is not ResourceType tipoSeleccionado ||
            MateriaPicker.SelectedItem is not SubjectOutputDTO materiaSeleccionada)
            {
                await DisplayAlert("Campos incompletos", "Por favor llena todos los campos obligatorios.", "OK");
                return;
            }

            var nuevoRecurso = new ResourcesItem
            {
                Title = TituloEntry.Text,
                Description = DescripcionEditor.Text,
                Link = RutaArchivoEntry.Text,
                UploadDate = DateTime.Now,
                ResourceTypeId = tipoSeleccionado.Id_Resource_Type
            };

            var recursoCreado = await _httpService.CreateResource(nuevoRecurso);
            if (recursoCreado == null || recursoCreado.Id == 0)
            {
                await DisplayAlert("Error", "El recurso no se pudo crear correctamente.", "OK");
                return;
            }

            // Crear relación materia-recurso
            // Crear relación materia-recurso
            var relacion = new SubjectResourceInputDTO
            {
                ID_Subject = materiaSeleccionada.Id_Subject,
                ID_Resource = recursoCreado.Id
            };

            var relResponse = await _httpService.CreateSubjectResource(relacion);

            if (!relResponse.IsSuccessStatusCode)
            {
                await DisplayAlert("Error", "El recurso se creó, pero no se pudo vincular a una materia", "OK");
                return;
            }

            await DisplayAlert("Éxito", "Recurso guardado correctamente", "OK");
            await Navigation.PopAsync();
        }
    }
}
