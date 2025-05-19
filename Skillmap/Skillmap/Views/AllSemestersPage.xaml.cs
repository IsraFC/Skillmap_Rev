using System.Threading.Tasks;
using SkillmapLib1.Models;
using Skillmap.Services;
using Skillmap.ViewModels;

namespace Skillmap.Views
{
    public partial class AllSemestersPage : ContentPage
    {
        private readonly AllSemestersViewModel _viewModel;

        private bool isNotStudent;
        public bool IsNotStudent { get => isNotStudent; set => isNotStudent = value; }

        public AllSemestersPage(List<SemesterItem> semesters)
        {
            InitializeComponent();
            var httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
            _viewModel = new AllSemestersViewModel(httpService);
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.CargarSemestresCommand.Execute(null);
        }

        private async void OnSemesterSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is SemesterItem selected)
            {
                await Navigation.PushAsync(new SemesterSubjectsPage(selected));
                ((CollectionView)sender).SelectedItem = null;
            }
        }

        private async void OnAgregarMateriaClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddSubjectPage());
        }

        private async void OnEditarMateriaClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditSubjectPage());
        }
    }
}
