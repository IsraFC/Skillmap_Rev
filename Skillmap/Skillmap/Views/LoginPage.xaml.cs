using Skillmap.Services;
using Microsoft.Maui.Controls;

namespace Skillmap.Views
{
    /// <summary>
    /// Vista encargada del inicio de sesión de usuarios en la plataforma.
    /// Permite autenticar al usuario e identificar si tiene rol de administrador.
    /// </summary>
    public partial class LoginPage : ContentPage
    {
        private bool isAdmin;

        /// <summary>
        /// Propiedad que indica si el usuario autenticado tiene el rol de administrador.
        /// </summary>
        public bool IsAdmin { get => isAdmin; set => isAdmin = value; }

        /// <summary>
        /// Constructor que inicializa los componentes visuales de la página.
        /// </summary>
        public LoginPage()
        {
            InitializeComponent();
        }
    }
}
