using IngredientsTracker.Database;
using IngredientsTracker.Helpers;
using Microsoft.Maui.Controls;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace IngredientsTracker.ViewModels
{
    public partial class DishListVM : BindableObject
    {
        private readonly ApiService _api;
        private readonly Random _rand = new Random();


        private static readonly Color[] _tileColors = new[]
        {
            Color.FromArgb("#6FFF5733"), // Salmon
            Color.FromArgb("#6FC70039"), // Baby Pink
            Color.FromArgb("#6F2548C2"), // Baby Blue
            Color.FromArgb("#AFDE1D1D"), // Pale Red
            Color.FromArgb("#6F23D40A"), // Pale Green
            Color.FromArgb("#6F2AEAEB"), // Sky Blue
        };
        public class DishTileModel
        {
            public DishModel Dish { get; set; }
            public Color TileColor { get; set; }
        }
        public ObservableCollection<DishTileModel> DishTiles { get; set; } // Change what is in DishModel when API response data is sorted

        // Property for the new dish name input
        private string _newDishName;
        public string NewDishName
        {
            get => _newDishName;
            set
            {
                _newDishName = value;
                OnPropertyChanged();
            }
        }

        // We need the blank no param constructor for the xaml file to be able to bind
        public DishListVM() { }
        public DishListVM(ApiService api)
        {
            _api = api;
            DishTiles = new ObservableCollection<DishTileModel>();
            LoadDishes();
        }

        private async Task LoadDishes()
        {
            DishTiles.Clear(); // Make sure main object to display is empty (no weird duplicates)

            string response = await _api.GetAllDishes();
            JObject responseData = JObject.Parse(response);
            foreach (JObject dish in responseData["dishes"]) {
                DishModel dishInfo = new DishModel
                {
                    Id = (int)dish["id"],
                    Name = (string)dish["name"],
                    User_Id = (int)dish["user_id"]
                };
                var randColour = _rand.Next(_tileColors.Length);
                DishTiles.Add(new DishTileModel
                {
                    Dish = dishInfo,
                    TileColor = _tileColors[randColour]
                });
            }
        }

        public async Task AddDish()
        {
            if (!string.IsNullOrEmpty(NewDishName))
            {
                string response = await _api.AddNewDish(NewDishName);
                // Check content not null, other wise return (i.e., return if content is null since we cant do anything with it.)
                // And likely means that we were un authorised, and forced to log out. Since there should always be some content back
                // Even if just a suuccess flag like {success: true/false}
                if(string.IsNullOrEmpty(response))
                {
                    return;
                }
                JObject responseData = JObject.Parse(response);
                bool success = (bool)responseData["success"];
                // If success = false, something went wrong that is not auth, so just display message
                if (!success)
                {
                    // error message
                    return;
                }

                var randColour = _rand.Next(_tileColors.Length);
                DishTiles.Add(new DishTileModel
                {
                    Dish = new DishModel
                    {
                        Id = (int)responseData["info"]["id"],
                        Name = (string)responseData["info"]["name"],
                        User_Id = (int)responseData["info"]["user_id"]
                    },
                    TileColor = _tileColors[randColour]
                });
                // Add the new dish to the ObservableCollection to update the UI
                //Dishes.Add(newDish);

                // Clear the input field
                NewDishName = string.Empty;
            }
        }

        private async Task ShowDishDetails(DishModel dish)
        {
            //await Application.Current.MainPage.Navigation.PushAsync(new DishIngredients(_db, dish));
        }

        private async Task DeleteDishAsync(DishModel dish)
        {
            //if (dish == null)
            //    return;
            //bool isConfirmed = await Application.Current.MainPage.DisplayAlert(
            //    "Delete Dish",
            //    $"Are you sure you want to delete {dish.Name}, {dish.Id}?",
            //    "Yes",
            //    "No");
            //if (isConfirmed)
            //{
            //    // TODO:  Also remove the amounts of ingredients that are used
            //    Dishes.Remove(dish);
            //    await _db.DeleteItemAsync(dish);
            //}
        }

    }
}
