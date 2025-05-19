using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Skillmap.ViewModels
{
    public partial class ResourcesDetailViewModel : ObservableObject
    {
        [ObservableProperty] private string titulo = string.Empty;
        [ObservableProperty] private string descripcion = string.Empty;
        [ObservableProperty] private string fechaSubida = string.Empty;
        [ObservableProperty] private string link = string.Empty;

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
