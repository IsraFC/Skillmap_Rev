using SkillmapLib1.Models.DTO.OutputDTO;
using Skillmap.Services;
using Skillmap.ViewModels;

namespace Skillmap.Views;

/// <summary>
/// Vista que muestra los recursos educativos asociados a una materia seleccionada.
/// Permite al usuario explorar los materiales disponibles filtrados por materia.
/// </summary>
public partial class SubjectResourcesPage : ContentPage
{
    /// <summary>
    /// Constructor que recibe la materia seleccionada y configura el ViewModel.
    /// </summary>
    /// <param name="subject">Materia cuyos recursos se desean visualizar.</param>
    public SubjectResourcesPage(SubjectOutputDTO subject)
    {
        InitializeComponent();

        if (BindingContext is SubjectResourcesViewModel vm)
        {
            vm.MateriaSeleccionada = subject;
            vm.CargarRecursosCommand.Execute(null);
        }
    }
}
