using Skillmap.Models;

namespace Skillmap.Pages
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
    }
}
