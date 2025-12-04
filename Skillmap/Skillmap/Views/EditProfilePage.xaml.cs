namespace Skillmap.Views
{
    /// <summary>
    /// Vista que permite editar el perfil del usuario actual o de otros usuarios si el rol es administrador.
    /// Se conecta con <c>EditProfileViewModel</c> para gestionar la lógica de edición, carga y eliminación.
    /// </summary>
    public partial class EditProfilePage : ContentPage
    {
        /// <summary>
        /// Constructor que inicializa los componentes visuales de la página.
        /// </summary>
        public EditProfilePage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Evento que se ejecuta cuando la página aparece.
        /// Invoca el comando <c>CargarDatosCommand</c> del ViewModel asociado para preparar la información a mostrar.
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
