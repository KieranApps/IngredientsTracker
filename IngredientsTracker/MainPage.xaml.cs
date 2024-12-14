using IngredientsTracker.Database;
using Microsoft.Extensions.Logging;

namespace IngredientsTracker
{
    public partial class MainPage : ContentPage
    {
        private readonly Database.Database _db;

        public MainPage(Database.Database db)
        {
            InitializeComponent();
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");
            _db = db;
        }
        
        private void ViewDishesList(object sender, EventArgs e)
        {
            Navigation.PushAsync(new DishList(_db));
        }

        private void ViewAllIngredients(object sender, EventArgs e)
        {
            Navigation.PushAsync(new IngredientsList(_db));
        }
    
        private void ViewSchedule(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Schedule(_db));
        }

        private void ViewShoppingList(object sender, EventArgs e)
        {
            //Navigation.PushAsync(new ShoppingList(_db));
        }
    }

}
