using IngredientsTracker.Helpers;
using IngredientsTracker.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;

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

            builder.Services.AddHttpClient("ApiService", client =>
            {
                // client.BaseAddress = new Uri(configData["ApiHost"]);
                client.Timeout = TimeSpan.FromSeconds(30);
            });
            builder.Services.AddSingleton<ApiService>();

            builder.Services.AddSingleton<MainPage>();

            builder.Services.AddTransient<CreateAccount>();
            builder.Services.AddTransient<CreateAccountVM>();


            builder.Services.AddTransient<Login>();
            builder.Services.AddTransient<LoginVM>();
            
            builder.Services.AddSingleton<HomePage>();

            builder.Services.AddTransient<DishList>();
            builder.Services.AddTransient<DishListVM>();

            builder.Services.AddTransient<DishInformation>();
            builder.Services.AddTransient<DishInformationVM>();

            // Remove after testing
            //TokenHandler th = new TokenHandler();
            //th.DeleteRefreshToken();
            //th.DeleteAccessToken();

            var app = builder.Build();

            App.ServiceProvider = app.Services;

            return app;
        }
    }
}
