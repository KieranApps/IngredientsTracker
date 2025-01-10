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
        }
    }
}
