using System.Threading.Tasks;
using SkillmapLib1.Models;
using Skillmap.Services;
using Skillmap.ViewModels;

namespace Skillmap.Views
{
    /// <summary>
    /// Vista que muestra todos los semestres disponibles junto con las materias asociadas a cada uno.
    /// Permite seleccionar un semestre, agregar nuevas materias o editar existentes.
    /// </summary>
    public partial class AllSemestersPage : ContentPage
    {
        private readonly AllSemestersViewModel _viewModel;

        private bool isNotStudent;

        /// <summary>
        /// Indica si el usuario actual tiene permisos de edición (no es estudiante).
        /// </summary>
        public bool IsNotStudent { get => isNotStudent; set => isNotStudent = value; }

        /// <summary>
        /// Constructor que inicializa los componentes y enlaza el ViewModel, recibiendo los semestres como parámetro.
        /// </summary>
        /// <param name="semesters">Lista de semestres a mostrar.</param>
        public AllSemestersPage(List<SemesterItem> semesters)
        {
            InitializeComponent();

            var httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
            _viewModel = new AllSemestersViewModel(httpService);
            BindingContext = _viewModel;
        }

        /// <summary>
        /// Evento que se ejecuta al aparecer la página.
        /// Carga o actualiza la lista de semestres visibles.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.CargarSemestresCommand.Execute(null);
        }

        /// <summary>
        /// Evento que se dispara al seleccionar un semestre de la lista.
        /// Navega a la vista de materias correspondientes a ese semestre.
        /// </summary>
        private async void OnSemesterSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is SemesterItem selected)
            {
                await Navigation.PushAsync(new SemesterSubjectsPage(selected));
                ((CollectionView)sender).SelectedItem = null;
            }
        }

        /// <summary>
        /// Navega a la página para agregar una nueva materia.
        /// Solo visible para administradores y docentes.
        /// </summary>
        private async void OnAgregarMateriaClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddSubjectPage());
        }

        /// <summary>
        /// Navega a la página para editar o eliminar materias existentes.
        /// </summary>
        private async void OnEditarMateriaClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditSubjectPage());
        }
    }
}
