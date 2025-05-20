using System.Threading.Tasks;
using Microsoft.Maui.Controls; // Importa los controles de .NET MAUI
using Skillmap.ViewModels; // Importa el espacio de nombres de los modelos de vista 

namespace Skillmap.Views // Define el espacio de nombres donde se encuentra la pantalla
{
    /// <summary>
    /// Vista principal de la aplicación SkillMap.
    /// Muestra al usuario sus semestres recientes y recursos recomendados.
    /// </summary>
    public partial class MainPage : ContentPage // Declara la clase MainPage, que representa la pantalla principal
    {
        /// <summary>
        /// Constructor por defecto que inicializa los componentes visuales de la página.
        /// </summary>
        public MainPage()
        {
            InitializeComponent(); // Inicializa los componentes definidos en MainPage
        }

        /// <summary>
        /// Evento que se ejecuta cuando la página aparece en pantalla.
        /// Ejecuta el comando de inicialización del ViewModel para cargar datos.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is MainViewModel vm)
            {
                vm.InicializarVistaCommand.Execute(null);
            }
        }
    }
}
