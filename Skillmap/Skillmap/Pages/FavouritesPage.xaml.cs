namespace Skillmap.Pages;
using Skillmap.Models;

public partial class FavoritesPage : ContentPage
{
    private List<ResourcesItem> allFavorites = new List<ResourcesItem>();
    /// <summary>
    /// Default constructor
    /// </summary>
    public FavoritesPage()
	{
		InitializeComponent();
        LoadFavorites();
	}

    /// <summary>
    /// Muestra los recursos guardados en los favoritos del usuario
    /// </summary>
    private void LoadFavorites()
    {
        // Datos de ejemplo
        allFavorites = new List<ResourcesItem>
        {
            new ResourcesItem { Title = "Curso de MAUI", Description = "Introducción a .NET MAUI", UploadDate = new DateOnly(2024,01,20)},
            new ResourcesItem { Title = "Fundamentos de C#", Description = "Aprende los conceptos básicos de C#", UploadDate = new DateOnly(2024,01,25)},
            new ResourcesItem { Title = "Desarrollo Web", Description = "HTML, CSS y JavaScript" , UploadDate = new DateOnly(2024,01,30)}
        };

        // Asignar datos a la lista
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
        favoritesCollectionView.ItemsSource = allFavorites.Where(f => f.Title.ToLower().Contains(searchText)).ToList();
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
            await Navigation.PushAsync(new ResourcesDetailPage(new ResourcesItem
            {
                Title = selectedFavorite.Title,
                Description = selectedFavorite.Description,
                Link = "https://www.example.com",
                UploadDate = selectedFavorite.UploadDate,
            }));
        }
    }
}