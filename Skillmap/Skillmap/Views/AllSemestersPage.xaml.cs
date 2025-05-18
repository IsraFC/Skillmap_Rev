using System.Threading.Tasks;
using Skillmap.Models;
using Skillmap.Services;

namespace Skillmap.Views
{
    public partial class AllSemestersPage : ContentPage
    {
        public AllSemestersPage(List<SemesterItem> semesters)
        {
            InitializeComponent();
            semestersCollectionView.ItemsSource = semesters;
        }

        private async void OnSemesterSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count == 0)
                return;

            var selectedSemester = (SemesterItem)e.CurrentSelection.FirstOrDefault();
            await Navigation.PushAsync(new SemesterSubjectsPage(selectedSemester));

            // Limpiar la selección
            ((CollectionView)sender).SelectedItem = null;
        }

        private async void OnAgregarMateriaClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CrearMateriaPage());
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                var httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
                var subjects = await httpService.GetSubjects();

                var grouped = subjects
                    .OrderBy(s => int.Parse(s.Semester.Split('°')[0]))
                    .GroupBy(s => s.Semester)
                    .Select(g => new SemesterItem
                    {
                        Name = g.Key,
                        Subjects = g.ToList()
                    })
                    .ToList();

                semestersCollectionView.ItemsSource = grouped;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudieron cargar los semestres: {ex.Message}", "OK");
            }
        }

        private async void OnEditarMateriaClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditarMateriaPage());
        }
    }
}
