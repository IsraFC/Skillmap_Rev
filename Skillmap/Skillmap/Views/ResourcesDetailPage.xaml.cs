namespace Skillmap.Views;

using System.Diagnostics;
using Skillmap.ViewModels;
using SkillmapLib1.Models;

public partial class ResourcesDetailPage : ContentPage
{
    private string resourceLink;
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