namespace Skillmap.Views;

using System.Diagnostics;
using Skillmap.ViewModels;
using SkillmapLib1.Models;

/// <summary>
/// Vista que muestra los detalles de un recurso educativo seleccionado.
/// Muestra el título, descripción, fecha de subida y enlace.
/// </summary>
public partial class ResourcesDetailPage : ContentPage
{
    private string resourceLink;

    /// <summary>
    /// Constructor que inicializa la vista y carga la información del recurso en el ViewModel.
    /// </summary>
    /// <param name="resource">Recurso que se desea visualizar en detalle.</param>
    public ResourcesDetailPage(ResourcesItem resource)
    {
        InitializeComponent();

        if (BindingContext is ResourcesDetailViewModel vm)
        {
            vm.Titulo = resource.Title;
            vm.Descripcion = resource.Description;
            vm.Link = resource.Link;
            vm.FechaSubida = $"Fecha de subida: {resource.UploadDate:dd/MM/yyyy}";
        }
    }
}
