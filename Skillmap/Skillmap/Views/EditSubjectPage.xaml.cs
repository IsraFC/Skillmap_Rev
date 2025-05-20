using Skillmap.Services;
using SkillmapLib1.Models.DTO.InputDTO;
using SkillmapLib1.Models.DTO.OutputDTO;

namespace Skillmap.Views
{
    /// <summary>
    /// Vista destinada a la edici�n o eliminaci�n de materias registradas en la plataforma.
    /// Se enlaza con <see cref="ViewModels.EditSubjectViewModel"/> para gestionar la l�gica.
    /// </summary>
    public partial class EditSubjectPage : ContentPage
    {
        /// <summary>
        /// Constructor que inicializa los componentes visuales de la vista.
        /// </summary>
        public EditSubjectPage()
        {
            InitializeComponent();
        }
    }
}
