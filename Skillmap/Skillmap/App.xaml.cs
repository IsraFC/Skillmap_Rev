using Microsoft.Maui.Controls; // Importa el espacio de nombres necesario para usar los controles de .NET MAUI
using Skillmap.Pages;

namespace Skillmap // Define el espacio de nombres del proyecto
{
    public partial class App : Application // Declara la clase App, que representa la aplicaci�n principal
    {
        public App() // Constructor de la aplicaci�n
        {
            InitializeComponent();// Inicializa los componentes de la aplicaci�n
        }

        // Correcci�n: No es necesario usar IActivationState
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new NavigationPage(new LoginPage()));
        }

        public void GoToMainApp()
        {
            // Al hacer login, cambia la pantalla principal a AppShell
            Application.Current.MainPage = new AppShell();
        }
    }
}
