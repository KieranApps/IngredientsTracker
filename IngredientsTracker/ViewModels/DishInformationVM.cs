using IngredientsTracker.Database;
using IngredientsTracker.Helpers;
using System.Collections.ObjectModel;


namespace IngredientsTracker.ViewModels
{
    public partial class DishInformationVM : BindableObject 
    {
        private readonly ApiService _api;
        public ObservableCollection<DishIngredientsList> Ingredients { get; set; }
        public DishModel CurrentDish;

        // Property for the new dish name input
        private string _newIngredient;
        public string NewIngredient
        {
            get => _newIngredient;
            set
            {
                _newIngredient = value;
                OnPropertyChanged();
            }
        }

        private string _newIngredientAmount; // Convert to float once focus off and check/validate
        public string NewIngredientAmount
        {
            get => _newIngredientAmount;
            set
            {
                _newIngredientAmount =  value;
                OnPropertyChanged();
            }
        }

        public DishInformationVM() { }
        public DishInformationVM(ApiService api)
        {
            _api = api;
            Ingredients = new ObservableCollection<DishIngredientsList>();
            LoadIngredients();
        }

        public void SetDish(DishModel dish)
        {
            CurrentDish = dish;
        }
        public async Task LoadIngredients()
        {

            
        }

        public async Task SubmitIngredientForDish()
        {
            
            
        }

        public async Task RemoveIngredientFromDish(DishIngredientsList ingredientDishLink)
        {
            
        }


        public async Task EditIngredientEntry(DishIngredientsList editedIng)
        {
            

        }
        
        private async void UpdateIngredientTotal()
        {

        }
    }
}
