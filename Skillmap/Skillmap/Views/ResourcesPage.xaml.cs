using SkillmapLib1.Models;
using Skillmap.Services;
using System.Threading.Tasks;
using Skillmap.ViewModels;

namespace Skillmap.Views;

/// <summary>
/// Vista que muestra todos los recursos educativos disponibles.
/// Permite a los usuarios filtrarlos y, si no son estudiantes, acceder a las opciones de agregar o editar recursos.
/// </summary>
public partial class ResourcesPage : ContentPage
{
    private bool isNotStudent;

    /// <summary>
    /// Indica si el usuario actual tiene permisos elevados (Admin o Docente).
    /// </summary>
    public bool IsNotStudent { get => isNotStudent; set => isNotStudent = value; }

    /// <summary>
    /// Constructor por defecto que inicializa los componentes visuales de la página.
    /// </summary>
    public ResourcesPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Evento que se ejecuta cuando la página aparece en pantalla.
    /// Invoca el comando del ViewModel para cargar los recursos disponibles.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ResourcesViewModel vm)
        {
            vm.CargarRecursosCommand.Execute(null);
        }
    }
}
