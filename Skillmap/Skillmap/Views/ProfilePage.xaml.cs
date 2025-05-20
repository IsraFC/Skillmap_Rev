namespace Skillmap.Views;
using Skillmap.ViewModels;

/// <summary>
/// Vista que muestra la informaci�n del perfil del usuario actual.
/// Desde aqu� se puede acceder a la edici�n del perfil, cierre de sesi�n y creaci�n de usuarios si es administrador.
/// </summary>
public partial class ProfilePage : ContentPage
{
    /// <summary>
    /// Constructor por defecto que inicializa los componentes visuales de la p�gina.
    /// </summary>
    public ProfilePage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Evento que se ejecuta cuando la p�gina aparece en pantalla.
    /// Invoca el comando para cargar los datos del perfil del usuario actual.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ProfileViewModel vm)
        {
            vm.CargarUsuarioCommand.Execute(null);
        }
    }
}
