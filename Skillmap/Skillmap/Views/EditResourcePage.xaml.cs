using Skillmap.ViewModels;
using SkillmapLib1.Models;
using SkillmapLib1.Models.DTO.InputDTO;
using Skillmap.Services;

namespace Skillmap.Views
{
    /// <summary>
    /// Vista que permite editar o eliminar recursos educativos existentes.
    /// Se enlaza con el <see cref="EditResourceViewModel"/> que gestiona la lógica de edición y actualización.
    /// </summary>
    public partial class EditResourcePage : ContentPage
    {
        /// <summary>
        /// Constructor que inicializa la vista y enlaza el ViewModel con el servicio HTTP.
        /// </summary>
        public EditResourcePage()
        {
            InitializeComponent();

            BindingContext = new EditResourceViewModel(
                (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService)));
        }
    }
}
