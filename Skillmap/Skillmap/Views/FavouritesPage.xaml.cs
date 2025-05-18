using Skillmap.Services;
using SkillmapLib1.Models;
using Microsoft.Maui.Controls;

namespace Skillmap.Views;

public partial class FavoritesPage : ContentPage
{
    private readonly HttpService _httpService;
    private List<ResourcesItem> allFavorites = new();

    /// <summary>
    /// Default constructor
    /// </summary>
    public FavoritesPage()
	{
		InitializeComponent();
        _httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadFavoritesDesdeApi();
    }

    /// <summary>
    /// Muestra los recursos guardados en los favoritos del usuario
    /// </summary>
    private async Task LoadFavoritesDesdeApi()
    {
        var recursos = await _httpService.GetResources();
        int cantidad = recursos.Count / 2;

        var random = new Random();
        allFavorites = recursos.OrderBy(x => random.Next()).Take(cantidad).ToList();

        favoritesCollectionView.ItemsSource = allFavorites;
    }

    /// <summary>
    /// Filtro de bisqueda en los favoritos
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = e.NewTextValue.ToLower();
        favoritesCollectionView.ItemsSource = allFavorites
            .Where(f => f.Title.ToLower().Contains(searchText))
            .ToList();
    }

    /// <summary>
    /// Muestra los detalles del recurso
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnViewMorePressed(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button?.BindingContext is ResourcesItem selectedFavorite)
        {
            await Navigation.PushAsync(new ResourcesDetailPage(selectedFavorite));
        }
    }
}