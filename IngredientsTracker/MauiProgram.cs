using IngredientsTracker.Helpers;
using IngredientsTracker.ViewModels;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.Json;

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
            var assembly = Assembly.GetExecutingAssembly();
            using Stream stream = assembly.GetManifestResourceStream("IngredientsTracker.Resources.config.json");
            using StreamReader reader = new StreamReader(stream);
            string json = reader.ReadToEnd();

            var configData = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            builder.Services.AddHttpClient("ApiService", client =>
            {
                client.BaseAddress = new Uri(configData["ApiHost"]);
                client.Timeout = TimeSpan.FromSeconds(30);
            });
            builder.Services.AddSingleton<ApiService>();

            builder.Services.AddSingleton<MainPage>();

            builder.Services.AddTransient<CreateAccount>();
            builder.Services.AddTransient<Login>();
            
            //builder.Services.AddSingleton<HomePage>();

            //builder.Services.AddTransient<DishList>();
            var app = builder.Build();

            App.ServiceProvider = app.Services;

            return app;
        }
    }
}
