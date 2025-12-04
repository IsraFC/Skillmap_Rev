using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Skillmap.ViewModels
{
    /// <summary>
    /// ViewModel para mostrar los detalles de un recurso educativo seleccionado.
    /// Permite visualizar su título, descripción, fecha y enlace, así como abrir el recurso en el navegador.
    /// </summary>
    public partial class ResourcesDetailViewModel : ObservableObject
    {
        /// <summary>
        /// Título del recurso educativo.
        /// </summary>
        [ObservableProperty] private string titulo = string.Empty;

        /// <summary>
        /// Descripción del recurso.
        /// </summary>
        [ObservableProperty] private string descripcion = string.Empty;

        /// <summary>
        /// Fecha en la que fue subido el recurso.
        /// </summary>
        [ObservableProperty] private string fechaSubida = string.Empty;

        /// <summary>
        /// Enlace al recurso (URL).
        /// </summary>
        [ObservableProperty] private string link = string.Empty;

        /// <summary>
        /// Comando para abrir el enlace del recurso en el navegador del dispositivo.
        /// Si no hay enlace disponible o si ocurre un error, se muestra un mensaje de alerta.
        /// </summary>
        [RelayCommand]
        private async Task AbrirLink()
        {
            if (!string.IsNullOrWhiteSpace(Link))
            {
                try
                {
                    await Launcher.OpenAsync(new Uri(Link));
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "No se pudo abrir el enlace.", "OK");
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Aviso", "Este recurso no tiene un enlace disponible.", "OK");
            }
        }
    }
}
