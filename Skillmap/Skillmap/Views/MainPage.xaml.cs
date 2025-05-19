using System.Threading.Tasks;
using Microsoft.Maui.Controls; // Importa los controles de .NET MAUI
using Skillmap.ViewModels; // Importa el espacio de nombres de los modelos de vista 

namespace Skillmap.Views // Define el espacio de nombres donde se encuentra la pantalla
{
    public partial class MainPage : ContentPage // Declara la clase MainPage, que representa la pantalla principal
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public MainPage()
        {
            InitializeComponent(); // Inicializa los componentes definidos en MainPage
        }

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
