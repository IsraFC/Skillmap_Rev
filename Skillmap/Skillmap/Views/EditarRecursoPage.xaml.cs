using SkillmapLib1.Models;
using Skillmap.Services;
using Microsoft.Maui.Controls;
using System.Linq;
using SkillmapLib1.Models.DTO.OutputDTO;
using SkillmapLib1.Models.DTO.InputDTO;

namespace Skillmap.Views;

public partial class EditarRecursoPage : ContentPage
{
    private readonly HttpService _httpService;
    private List<ResourcesItem> _recursos = new();
    private List<SubjectOutputDTO> _materias = new();
    private List<ResourceType> _tipos = new();
    private List<SubjectResource> _subjectResources = new();
    private ResourcesItem? _recursoSeleccionado;

    public EditarRecursoPage()
	{
		InitializeComponent();
        _httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Cargar recursos
        _recursos = await _httpService.GetResources();
        RecursoPicker.ItemsSource = _recursos;
        RecursoPicker.ItemDisplayBinding = new Binding("Title");

        // Cargar tipos
        _tipos = await _httpService.GetResourceTypes();
        TipoRecursoPicker.ItemsSource = _tipos;
        TipoRecursoPicker.ItemDisplayBinding = new Binding("Id_Resource_Type");

        // Cargar materias
        _materias = await _httpService.GetSubjects();
        MateriaPicker.ItemsSource = _materias;
        MateriaPicker.ItemDisplayBinding = new Binding("Name");

        // Cargar relaciones
        _subjectResources = await _httpService.GetAllSubjectResources();
    }

    private void OnRecursoSeleccionado(object sender, EventArgs e)
    {
        if (RecursoPicker.SelectedIndex == -1)
            return;

        _recursoSeleccionado = (ResourcesItem)RecursoPicker.SelectedItem;

        TituloEntry.Text = _recursoSeleccionado.Title;
        DescripcionEditor.Text = _recursoSeleccionado.Description;
        RutaArchivoEntry.Text = _recursoSeleccionado.Link;
        EsPublicoCheckBox.IsChecked = true;

        TipoRecursoPicker.SelectedIndex = _tipos
            .FindIndex(r => r.Id_Resource_Type == _recursoSeleccionado.ResourceTypeId);

        var relacion = _subjectResources
            .FirstOrDefault(sr => sr.ID_Resource == _recursoSeleccionado.Id);

        if (relacion != null)
        {
            MateriaPicker.SelectedIndex = _materias
                .FindIndex(m => m.Id_Subject == relacion.ID_Subject);
        }
        else
        {
            MateriaPicker.SelectedIndex = -1;
        }
    }

    private async void OnActualizarRecursoClicked(object sender, EventArgs e)
    {
        if (_recursoSeleccionado == null)
            return;

        var selectedTipo = TipoRecursoPicker.SelectedItem as ResourceType;
        var selectedMateria = MateriaPicker.SelectedItem as SubjectOutputDTO;

        var recursoActualizado = new ResourcesItem
        {
            Id = _recursoSeleccionado.Id,
            Title = TituloEntry.Text,
            Description = DescripcionEditor.Text,
            Link = RutaArchivoEntry.Text,
            UploadDate = DateTime.Now,
            ResourceTypeId = selectedTipo?.Id_Resource_Type ?? ""
        };

        // Actualizar el recurso
        var res = await _httpService.UpdateResource(recursoActualizado.Id, recursoActualizado);

        if (res.IsSuccessStatusCode)
        {
            var relacionExistente = _subjectResources.FirstOrDefault(sr => sr.ID_Resource == _recursoSeleccionado.Id);
            if (relacionExistente != null)
            {
                await _httpService.DeleteSubjectResource(relacionExistente.ID_Subject, relacionExistente.ID_Resource);
            }

            await _httpService.CreateSubjectResource(new SubjectResourceInputDTO
            {
                ID_Subject = selectedMateria!.Id_Subject,
                ID_Resource = recursoActualizado.Id
            });

            await DisplayAlert("Éxito", "Recurso actualizado correctamente", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Error", "No se pudo actualizar el recurso", "OK");
        }
    }

    private async void OnEliminarRecursoClicked(object sender, EventArgs e)
    {
        if (_recursoSeleccionado == null)
            return;

        bool confirm = await DisplayAlert("¿Estás seguro?", "Esto eliminará el recurso permanentemente.", "Sí", "Cancelar");
        if (!confirm) return;

        var relacion = _subjectResources.FirstOrDefault(r => r.ID_Resource == _recursoSeleccionado.Id);

        if (relacion != null)
        {
            await _httpService.DeleteSubjectResource(relacion.ID_Subject, relacion.ID_Resource);
        }

        await _httpService.DeleteResource(_recursoSeleccionado.Id);
        await DisplayAlert("Eliminado", "Recurso eliminado correctamente", "OK");
        await Navigation.PopAsync();
    }
}