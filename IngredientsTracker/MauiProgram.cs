using IngredientsTracker.Helpers;
using IngredientsTracker.ViewModels;
using Microsoft.Extensions.Logging;

namespace IngredientsTracker
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
            // Load host

            builder.Services.AddHttpClient("ApiService", client =>
            {
                client.BaseAddress = new Uri("https://example.com/api/");
                client.Timeout = TimeSpan.FromSeconds(30);
            });
            builder.Services.AddSingleton<ApiService>();

            builder.Services.AddSingleton<MainPage>();
            
            //builder.Services.AddSingleton<HomePage>();

            //builder.Services.AddTransient<DishList>();
            var app = builder.Build();

            App.ServiceProvider = app.Services;

            return app;
        }
    }
}
