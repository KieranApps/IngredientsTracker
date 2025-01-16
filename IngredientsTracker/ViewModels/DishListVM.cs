using IngredientsTracker.Database;
using IngredientsTracker.Helpers;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace IngredientsTracker.ViewModels
{
    public partial class DishListVM : BindableObject
    {
        private readonly ApiService _api;
        public ObservableCollection<DishModel> Dishes { get; set; } // Change what is in DishModel when API response data is sorted

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
            Dishes = new ObservableCollection<DishModel>();
            LoadDishes();
        }

        private async Task LoadDishes()
        {
            
            //Dishes.Clear(); // Make sure main object to display is empty (no weird duplicates)

            //// Populate the ObservableCollection
            //foreach (var dish in dishes)
            //{
            //    Dishes.Add(dish);
            //}
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


                // If success = false, something went wrong that is not auth, so just display message

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
