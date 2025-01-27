using IngredientsTracker.Helpers;
using System.Diagnostics;

namespace IngredientsTracker
{
    public partial class HomePage : ContentPage
    {
        private UserService _userService;

        public HomePage()
        {
            InitializeComponent();
            _userService = new UserService();
        }
        private async void LoadUserInfo(object sender, EventArgs e)
        {
            string name = await _userService.GetUserName();
            WelcomeLabel.Text = "Welcome, " + name + "!";
        }

        private void ViewDishesList(object sender, EventArgs e)
        {
            var dishList = App.ServiceProvider.GetService<DishList>();
            Navigation.PushAsync(dishList);
        }
        private void ViewSchedule(object sender, EventArgs e)
        {
            var schedule = App.ServiceProvider.GetService<Schedule>();
            Navigation.PushAsync(schedule);
        }

        //private void ViewAllIngredients(object sender, EventArgs e)
        //{
        //    Navigation.PushAsync(new IngredientsList(_db));
        //}


        //private void ViewShoppingList(object sender, EventArgs e)
        //{
        //    //Navigation.PushAsync(new ShoppingList(_db));
        //}
    }

}
