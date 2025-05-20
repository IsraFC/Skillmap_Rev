namespace Skillmap.Views
{
    /// <summary>
    /// Vista que permite editar el perfil del usuario actual o de otros usuarios si el rol es administrador.
    /// Se conecta con <c>EditProfileViewModel</c> para gestionar la l�gica de edici�n, carga y eliminaci�n.
    /// </summary>
    public partial class EditProfilePage : ContentPage
    {
        /// <summary>
        /// Constructor que inicializa los componentes visuales de la p�gina.
        /// </summary>
        public EditProfilePage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Evento que se ejecuta cuando la p�gina aparece.
        /// Invoca el comando <c>CargarDatosCommand</c> del ViewModel asociado para preparar la informaci�n a mostrar.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is ViewModels.EditProfileViewModel vm)
            {
                vm.CargarDatosCommand.Execute(null);
            }
        }
    }
}
