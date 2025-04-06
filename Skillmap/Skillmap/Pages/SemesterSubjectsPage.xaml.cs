using Skillmap.Models;

namespace Skillmap.Pages;

public partial class SemesterSubjectsPage : ContentPage
{
    /// <summary>
    /// Constructor por defecto de pagina de materias
    /// </summary>
    /// <param name="semester"></param>
	public SemesterSubjectsPage(SemesterItem semester)
	{
		InitializeComponent();
        SemesterTitle.Text = semester.Name;
        subjectsCollectionView.ItemsSource = semester.Subjects;
    }
}