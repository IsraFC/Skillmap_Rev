using Microsoft.Extensions.Logging;
using Skillmap.Services;
using Skillmap.Views;

namespace Skillmap
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            //  HttpClient configurado para apuntar al Web API de Skillmap
            builder.Services.AddHttpClient<HttpService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7163/"); // Cambia por la IP real de tu API
            });

            // ✅ Registro de páginas y sus ViewModels
            builder.Services
                .AddTransient<LoginPage>()
                .AddTransient<MainPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
