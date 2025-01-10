using SQLite;

namespace IngredientsTracker
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; set; }
        public App()
        {
            InitializeComponent();
            //this.UserAppTheme = AppTheme.Light;
            var mainPage = ServiceProvider.GetService<MainPage>();
            MainPage = new NavigationPage(mainPage);


            // How to get other pages from witihin the MainPage and onwards...
            //var secondPage = App.Current.Services.GetService<SecondPage>();
            //await Navigation.PushAsync(secondPage);
        }
    }
}
