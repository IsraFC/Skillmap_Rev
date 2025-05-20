using System.Threading.Tasks;
using SkillmapLib1.Models;
using Skillmap.ViewModels;

namespace Skillmap.Views;

/// <summary>
/// Vista que muestra las materias correspondientes al semestre seleccionado.
/// Permite navegar a los recursos de una materia al seleccionarla.
/// </summary>
public partial class SemesterSubjectsPage : ContentPage
{
    /// <summary>
    /// Constructor que recibe el semestre seleccionado y configura el ViewModel con su nombre.
    /// </summary>
    /// <param name="semester">Objeto <see cref="SemesterItem"/> que representa el semestre seleccionado.</param>
    public SemesterSubjectsPage(SemesterItem semester)
    {
        InitializeComponent();

        if (BindingContext is SemesterSubjectsViewModel vm)
        {
            vm.NombreSemestre = semester.Name;
            vm.CargarMateriasCommand.Execute(null);
        }
    }

    /// <summary>
    /// Evento que se dispara al seleccionar una materia de la lista.
    /// Asigna la materia al ViewModel y ejecuta la navegación a sus recursos.
    /// </summary>
    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (BindingContext is ViewModels.SemesterSubjectsViewModel vm &&
            e.CurrentSelection.FirstOrDefault() is SkillmapLib1.Models.DTO.OutputDTO.SubjectOutputDTO selected)
        {
            vm.MateriaSeleccionada = selected;
            vm.MateriaCommand.Execute(null);
        }

        // Deselecciona el elemento para permitir futuras selecciones
        ((CollectionView)sender).SelectedItem = null;
    }
}
