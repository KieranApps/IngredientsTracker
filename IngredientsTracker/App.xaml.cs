using SQLite;

namespace IngredientsTracker
{
    public partial class App : Application
    {
        private readonly Database.Database _db;
        public App()
        {
            InitializeComponent();
            this.UserAppTheme = AppTheme.Light;

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");
            _db = new Database.Database(dbPath);

            MainPage = new NavigationPage(new MainPage(_db));
        }
    }
}
