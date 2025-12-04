using SkillmapLib1.Models.DTO.InputDTO;
using Skillmap.Services;
using SkillmapLib1.Models.DTO.OutputDTO;

namespace Skillmap.Views;

/// <summary>
/// Vista encargada de permitir a los administradores o docentes agregar una nueva materia.
/// Se conecta visualmente con el ViewModel correspondiente.
/// </summary>
public partial class AddSubjectPage : ContentPage
{
    /// <summary>
    /// Constructor que inicializa los componentes de la vista.
    /// </summary>
    public AddSubjectPage()
    {
        InitializeComponent();
    }
}
