using Skillmap.Services;
using SkillmapLib1.Models;
using Microsoft.Maui.Controls;
using Skillmap.ViewModels;

namespace Skillmap.Views;

public partial class FavoritesPage : ContentPage
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public FavoritesPage()
	{
		InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is FavoritesViewModel vm)
        {
            vm.CargarFavoritosCommand.Execute(null);
        }
    }
}