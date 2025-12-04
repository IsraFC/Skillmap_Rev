using Skillmap.Services;
using SkillmapLib1.Models;
using Microsoft.Maui.Controls;
using Skillmap.ViewModels;

namespace Skillmap.Views
{
    /// <summary>
    /// Vista que muestra una selección de recursos favoritos al usuario.
    /// Carga una lista de recursos para simular recomendaciones.
    /// </summary>
    public partial class FavoritesPage : ContentPage
    {
        /// <summary>
        /// Constructor que inicializa la vista y sus componentes visuales.
        /// </summary>
        public FavoritesPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Evento que se dispara cuando la página aparece en pantalla.
        /// Ejecuta el comando <c>CargarFavoritosCommand</c> del ViewModel para obtener los recursos.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is FavoritesViewModel vm)
            {
                vm.CargarFavoritosCommand.Execute(null);
            }
        }
    }
}
