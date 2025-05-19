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
            //builder.Services.AddHttpClient<HttpService>(client =>
            //{
            //    client.BaseAddress = new Uri("https://localhost:7163/");
            //});

            builder.Services.AddSingleton<HttpClient>(sp =>
            {
                return new HttpClient
                {
                    BaseAddress = new Uri("http://192.168.1.125:7163/")
                };
            });


            // Registrar HttpService como Singleton para que mantenga el token
            builder.Services.AddSingleton<HttpService>();

            // Registro de páginas y sus ViewModels
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
