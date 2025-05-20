using Microsoft.Extensions.Logging;
using Skillmap.Services;
using Skillmap.Views;

namespace Skillmap
{
    /// <summary>
    /// Clase responsable de configurar y construir la aplicación MAUI.
    /// Registra servicios, páginas y configuración general como fuentes y logging.
    /// </summary>
    public static class MauiProgram
    {
        /// <summary>
        /// Crea y configura la aplicación MAUI, incluyendo inyección de dependencias y servicios HTTP.
        /// </summary>
        /// <returns>Instancia construida de <see cref="MauiApp"/>.</returns>
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            // Configura la app principal
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            /// <summary>
            /// Configura HttpClient para apuntar a la dirección del Web API.
            /// Este cliente se inyecta a HttpService y es compartido por toda la aplicación.
            /// </summary>
            builder.Services.AddSingleton<HttpClient>(sp =>
            {
                return new HttpClient
                {
                    BaseAddress = new Uri("http://172.16.6.184:7163/")
                };
            });

            /// <summary>
            /// Registra <see cref="HttpService"/> como singleton para mantener el token y reutilizar el cliente.
            /// </summary>
            builder.Services.AddSingleton<HttpService>();

            /// <summary>
            /// Registro de páginas y su inyección de dependencias para navegación.
            /// </summary>
            builder.Services
                .AddTransient<LoginPage>()
                .AddTransient<MainPage>()
                .AddTransient<AddSubjectPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
