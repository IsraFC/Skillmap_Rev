namespace Skillmap.Pages;

using System.Diagnostics;
using Skillmap.Models;

public partial class ResourcesDetailPage : ContentPage
{
    private string resourceLink;
    public ResourcesDetailPage(ResourcesItem resource)
	{
		InitializeComponent();

        // Asigna los valores del recurso a los elementos de la UI
        titleLabel.Text = resource.Title;
        descriptionLabel.Text = resource.Description;
        uploadDateLabel.Text = $"Fecha de subida: {resource.UploadDate:dd/MM/yyyy}";
        resourceLink = resource.Link;
    }

    private async void OnOpenLinkClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(resourceLink))
        {
            try
            {
                await Launcher.OpenAsync(new Uri(resourceLink));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al abrir el enlace: {ex.Message}");
                await DisplayAlert("Error", "No se pudo abrir el enlace.", "OK");
            }
        }
        else
        {
            await DisplayAlert("Aviso", "Este recurso no tiene un enlace disponible.", "OK");
        }
    }
}