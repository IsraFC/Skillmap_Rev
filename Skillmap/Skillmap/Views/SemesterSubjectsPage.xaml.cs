using System.Threading.Tasks;
using SkillmapLib1.Models;
using Skillmap.ViewModels;

namespace Skillmap.Views;

public partial class SemesterSubjectsPage : ContentPage
{
    /// <summary>
    /// Constructor por defecto de pagina de materias
    /// </summary>
    /// <param name="semester"></param>
	public SemesterSubjectsPage(SemesterItem semester)
	{
		InitializeComponent();
        if (BindingContext is SemesterSubjectsViewModel vm)
        {
            vm.NombreSemestre = semester.Name;
            vm.CargarMateriasCommand.Execute(null);
        }
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (BindingContext is ViewModels.SemesterSubjectsViewModel vm &&
            e.CurrentSelection.FirstOrDefault() is SkillmapLib1.Models.DTO.OutputDTO.SubjectOutputDTO selected)
        {
            vm.MateriaSeleccionada = selected;
            vm.MateriaCommand.Execute(null);
        }

        ((CollectionView)sender).SelectedItem = null;
    }
}