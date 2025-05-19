using Skillmap.Services;
using SkillmapLib1.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Skillmap.ViewModels
{
    public partial class FavoritesViewModel : ObservableObject
    {
        private readonly HttpService _httpService;

        [ObservableProperty]
        private ObservableCollection<ResourcesItem> favoritosFiltrados = new();

        private List<ResourcesItem> todosLosFavoritos = new();

        [ObservableProperty]
        private string busquedaTexto = string.Empty;

        public ICommand VerMasCommand => new AsyncRelayCommand<ResourcesItem>(VerDetalles);

        public FavoritesViewModel()
        {
            _httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
            _ = CargarFavoritos();
        }

        partial void OnBusquedaTextoChanged(string value)
        {
            AplicarFiltro();
        }

        [RelayCommand]
        private async Task CargarFavoritos()
        {
            var recursos = await _httpService.GetResources();
            int cantidad = recursos.Count / 2;
            var random = new Random();
            todosLosFavoritos = recursos.OrderBy(x => random.Next()).Take(cantidad).ToList();

            AplicarFiltro();
        }

        private void AplicarFiltro()
        {
            if (string.IsNullOrWhiteSpace(BusquedaTexto))
            {
                FavoritosFiltrados = new ObservableCollection<ResourcesItem>(todosLosFavoritos);
            }
            else
            {
                var texto = BusquedaTexto.ToLower();
                var filtrados = todosLosFavoritos
                    .Where(f => f.Title.ToLower().Contains(texto))
                    .ToList();

                FavoritosFiltrados = new ObservableCollection<ResourcesItem>(filtrados);
            }
        }

        private async Task VerDetalles(ResourcesItem recurso)
        {
            await Shell.Current.Navigation.PushAsync(new Views.ResourcesDetailPage(recurso));
        }
    }
}
