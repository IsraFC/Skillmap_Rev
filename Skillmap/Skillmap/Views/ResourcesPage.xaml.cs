using SkillmapLib1.Models;
using Skillmap.Services;
using System.Threading.Tasks;

namespace Skillmap.Views;

public partial class ResourcesPage : ContentPage
{
    private readonly HttpService _httpService;
    private List<ResourcesItem> allResources = new();

	/// <summary>
	/// Default constructor
	/// </summary>
	public ResourcesPage()
	{
		InitializeComponent();
        _httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadResourcesFromApi();
    }

    private async Task LoadResourcesFromApi()
    {
        try
        {
            allResources = await _httpService.GetResources();
            resourcesCollectionView.ItemsSource = allResources;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudieron cargar los recursos: {ex.Message}", "OK");
        }
    }
    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = e.NewTextValue.ToLower();
        resourcesCollectionView.ItemsSource = allResources
            .Where(r => r.Title.ToLower().Contains(searchText))
            .ToList();
    }

    private async void OnViewMorePressed(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button?.BindingContext is ResourcesItem selectedFavorite)
        {
            var detalle = new ResourcesItem
            {
                Title = selectedFavorite.Title,
                Description = selectedFavorite.Description,
                Link = selectedFavorite.Link,
                UploadDate = selectedFavorite.UploadDate
            };

            await Navigation.PushAsync(new ResourcesDetailPage(detalle));
        }
    }

    private async void OnAddResourceButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CrearRecursoPage());
    }

    private async void OnEditFavoritesButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditarRecursoPage());
    }
}