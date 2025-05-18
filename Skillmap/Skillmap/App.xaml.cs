using Skillmap.Views;

namespace Skillmap // Define el espacio de nombres del proyecto
{
    public partial class App : Application // Declara la clase App, que representa la aplicaci�n principal
    {
        public IServiceProvider ServiceProvider { get; }

        public App(IServiceProvider serviceProvider) // Constructor de la aplicaci�n
        {
            InitializeComponent();// Inicializa los componentes de la aplicaci�n
            ServiceProvider = serviceProvider;
        }

        // Correcci�n: No es necesario usar IActivationState
        protected override Window CreateWindow(IActivationState? activationState)
        {
            var loginPage = ServiceProvider.GetRequiredService<LoginPage>();
            return new Window(new NavigationPage(loginPage));
        }

        public void GoToMainApp()
        {
            // Al hacer login, cambia la pantalla principal a AppShell
            var mainPage = ServiceProvider.GetRequiredService<MainPage>();
            Application.Current.MainPage = new AppShell();
        }
    }
}
