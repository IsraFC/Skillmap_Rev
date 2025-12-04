using Skillmap.Services;
using SkillmapLib1.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Skillmap.ViewModels
{
    /// <summary>
    /// ViewModel responsable de mostrar una lista de recursos marcados como favoritos.
    /// Incluye funciones para filtrado en tiempo real y navegación a detalles.
    /// </summary>
    public partial class FavoritesViewModel : ObservableObject
    {
        private readonly HttpService _httpService;

        /// <summary>
        /// Colección observable con los recursos favoritos filtrados según el texto de búsqueda.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<ResourcesItem> favoritosFiltrados = new();

        /// <summary>
        /// Lista completa de recursos favoritos sin filtrar.
        /// </summary>
        private List<ResourcesItem> todosLosFavoritos = new();

        /// <summary>
        /// Texto ingresado para realizar la búsqueda entre los favoritos.
        /// </summary>
        [ObservableProperty]
        private string busquedaTexto = string.Empty;

        /// <summary>
        /// Comando para navegar a la vista de detalles de un recurso.
        /// </summary>
        public ICommand VerMasCommand => new AsyncRelayCommand<ResourcesItem>(VerDetalles);

        /// <summary>
        /// Constructor que inicializa el ViewModel y carga los recursos favoritos.
        /// </summary>
        public FavoritesViewModel()
        {
            _httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
            _ = CargarFavoritos();
        }

        /// <summary>
        /// Evento que se dispara cuando cambia el texto de búsqueda.
        /// Aplica automáticamente el filtro a los recursos favoritos.
        /// </summary>
        /// <param name="value">Nuevo texto ingresado en el campo de búsqueda.</param>
        partial void OnBusquedaTextoChanged(string value)
        {
            AplicarFiltro();
        }

        /// <summary>
        /// Carga una lista aleatoria de recursos desde la API simulando los "favoritos".
        /// Posteriormente aplica el filtro en base al texto de búsqueda.
        /// </summary>
        [RelayCommand]
        private async Task CargarFavoritos()
        {
            var recursos = await _httpService.GetResources();

            // Simulación de favoritos: se toma la mitad de los recursos de forma aleatoria
            int cantidad = recursos.Count / 2;
            var random = new Random();
            todosLosFavoritos = recursos.OrderBy(x => random.Next()).Take(cantidad).ToList();

            AplicarFiltro();
        }

        /// <summary>
        /// Aplica un filtro de texto a la lista de favoritos y actualiza la colección observable.
        /// </summary>
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

        /// <summary>
        /// Navega a la vista de detalles del recurso seleccionado.
        /// </summary>
        /// <param name="recurso">Recurso al que se desea acceder en detalle.</param>
        private async Task VerDetalles(ResourcesItem recurso)
        {
            await Shell.Current.Navigation.PushAsync(new Views.ResourcesDetailPage(recurso));
        }
    }
}
