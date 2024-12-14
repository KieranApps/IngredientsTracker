using IngredientsTracker.Database;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace IngredientsTracker.ViewModels
{
    public partial class DishListVM : BindableObject
    {
        private readonly Database.Database _db;
        public ObservableCollection<DishModel> Dishes { get; set; }

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
        public DishListVM(Database.Database db)
        {
            _db = db;
            Dishes = new ObservableCollection<DishModel>();
            LoadDishesAsync();
        }

        // Command to save the new dish
        public ICommand SubmitDishCommand => new Command(async () => await AddDishAsync());
        public ICommand DeleteDishCommand => new Command<DishModel>(async (DishModel dish) => await DeleteDishAsync(dish));
        public ICommand ShowDishDetailsCommand => new Command<DishModel>(async (DishModel dish) => await ShowDishDetails(dish));

        private async Task LoadDishesAsync()
        {
            var dishes = await _db.GetItemsAsync<DishModel>();
            Dishes.Clear(); // Make sure main object to display is empty (no weird duplicates)

            // Populate the ObservableCollection
            foreach (var dish in dishes)
            {
                Dishes.Add(dish);
            }
        }

        private async Task AddDishAsync()
        {
            if (!string.IsNullOrEmpty(NewDishName))
            {
                // Insert into this table (DishModel), this data ( { Name: NewDishName } )
                var newDish = new DishModel { Name = NewDishName };
                await _db.SaveItemAsync(newDish);

                // Add the new dish to the ObservableCollection to update the UI
                Dishes.Add(newDish);

                // Clear the input field
                NewDishName = string.Empty;
            }
        }

        private async Task ShowDishDetails(DishModel dish)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new DishIngredients(_db, dish));
        }

        private async Task DeleteDishAsync(DishModel dish)
        {
            if (dish == null)
                return;
            bool isConfirmed = await Application.Current.MainPage.DisplayAlert(
                "Delete Dish",
                $"Are you sure you want to delete {dish.Name}, {dish.Id}?",
                "Yes",
                "No");
            if (isConfirmed)
            {
                // TODO:  Also remove the amounts of ingredients that are used
                Dishes.Remove(dish);
                await _db.DeleteItemAsync(dish);
            }
        }

    }
}
