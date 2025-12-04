using SkillmapLib1.Models;
using Skillmap.Services;
using SkillmapLib1.Models.DTO.OutputDTO;
using SkillmapLib1.Models.DTO.InputDTO;
using System.Net.Http.Json;
using Microsoft.Maui.Controls.Shapes;
using Skillmap.ViewModels;

namespace Skillmap.Views
{
    /// <summary>
    /// Vista para agregar un nuevo recurso educativo.
    /// Se enlaza con el ViewModel AddResourceViewModel mediante binding.
    /// </summary>
    public partial class AddResourcePage : ContentPage
    {
        /// <summary>
        /// Instancia del ViewModel encargado de la lógica de esta vista.
        /// </summary>
        private readonly AddResourceViewModel viewModel;

        /// <summary>
        /// Constructor que inicializa los componentes de la vista y vincula el ViewModel.
        /// </summary>
        public AddResourcePage()
        {
            InitializeComponent();

            // Se obtiene el servicio HTTP desde el contenedor de dependencias
            var httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));

            // Se establece el binding context con el ViewModel
            BindingContext = viewModel = new AddResourceViewModel(httpService);
        }

        /// <summary>
        /// Evento que se ejecuta al mostrar la página.
        /// Se encarga de cargar los datos necesarios para los pickers (materias y tipos de recurso).
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await viewModel.CargarDatosAsync();
        }
    }
}
