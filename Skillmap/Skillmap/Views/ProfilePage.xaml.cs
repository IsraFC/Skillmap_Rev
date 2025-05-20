namespace Skillmap.Views;
using Skillmap.ViewModels;

/// <summary>
/// Vista que muestra la información del perfil del usuario actual.
/// Desde aquí se puede acceder a la edición del perfil, cierre de sesión y creación de usuarios si es administrador.
/// </summary>
public partial class ProfilePage : ContentPage
{
    /// <summary>
    /// Constructor por defecto que inicializa los componentes visuales de la página.
    /// </summary>
    public ProfilePage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Evento que se ejecuta cuando la página aparece en pantalla.
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
