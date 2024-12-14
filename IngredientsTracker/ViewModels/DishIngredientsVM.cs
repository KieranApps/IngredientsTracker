using __XamlGeneratedCode__;
using IngredientsTracker.Database;
using IngredientsTracker.QueryClassDefs;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Xml.Linq;

namespace IngredientsTracker.ViewModels
{
    public partial class DishIngredientsVM : BindableObject 
    {
        private readonly Database.Database _db;
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

        public DishIngredientsVM() { }
        public DishIngredientsVM (Database.Database db, DishModel dish)
        {
            _db = db;
            CurrentDish = dish;
            Ingredients = new ObservableCollection<DishIngredientsList>();
            // Load the dish ingredients
            LoadIngredients(dish);
        }

        public ICommand SubmitIngredientCommand => new Command(async () => await SubmitIngredientForDish());
        public ICommand RemoveIngredientFromDishCommand => new Command<DishIngredientsList>(async (DishIngredientsList ingredientDishLink) => await RemoveIngredientFromDish(ingredientDishLink));

        public async Task LoadIngredients(DishModel dish)
        {

            var ingredients = await _db.GetIngredientsForDish(dish.Id);

            Ingredients.Clear(); // Ensure empty

            foreach (var ingredient in ingredients)
            {
                Ingredients.Add(ingredient);
            }
        }

        public async Task SubmitIngredientForDish()
        {
            try
            {
                foreach (var ing in Ingredients)
                {
                    if (ing.Name.ToLower() == NewIngredient.ToLower())
                    {
                        await Application.Current.MainPage.DisplayAlert($"{NewIngredient} Exists", "This ingredient exists in this dish. " +
                            "If you wish to edit the amount, you can do so on the existing entry.", "OK");
                        return; // Already exists in list
                    }
                }

                float IngredientAmount = float.Parse(NewIngredientAmount);
                IngredientsModel exists = await CheckForIngredient(NewIngredient);
                int ingId; // We need the ID of the existing or new ingredient to add to the dishIngredient table
                if (exists == null) // If not exists, add to ingredients then continue and add to the ingrediesnt dish list
                {
                    IngredientsModel newIng = new IngredientsModel{ 
                        Name = NewIngredient,
                        Amount = IngredientAmount,
                        TotalAmount = IngredientAmount // Just default to whats in for now
                    };
                    await _db.SaveItemAsync(newIng);
                    // We need to read the new ingredient to get the ID unfortunately
                    IngredientsModel i = await _db.GetIngredientByName(NewIngredient);
                    ingId = i.Id;
                } else
                {
                    ingId = exists.Id;
                }

                // Save the dish and ingredient pair to the table
                IngredientDishModel ingDishLink = new IngredientDishModel{
                    DishId = CurrentDish.Id,
                    IngredientId = ingId,
                    Amount = IngredientAmount
                };
                // Now we code to check and or edit the existing link between the two (mainly this is hte amount)

                // TODO: Update the total ingredient amount after new addition

                await _db.SaveItemAsync(ingDishLink);

                // Reset the input fields and add to the list
                DishIngredientsList ingToDisplay = new DishIngredientsList()
                {
                    Name = NewIngredient,
                    Amount = IngredientAmount,
                    DishId = CurrentDish.Id,
                    IngredientId = ingId
                };

                Ingredients.Add(ingToDisplay);
                if (exists != null)
                {
                    // Update total if already exists
                    IngredientsModel ing = await _db.GetIngredientByName(exists.Name);
                    float newTotal = exists.TotalAmount + IngredientAmount;
                    await _db.UpdateIngredientAmounts(exists.Id, newTotal);
                }
                NewIngredient = string.Empty;
                NewIngredientAmount = null;
            } catch(Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("An Error Occurred", $"Details: {e.Message}", "OK");
            }
            
        }

        public async Task RemoveIngredientFromDish(DishIngredientsList ingredientDishLink)
        {
            await _db.RemoveDishIngLink(ingredientDishLink.DishId, ingredientDishLink.IngredientId);

            // TODO: Update the amount of ingredient total in ingredients table
            IngredientsModel ing = await _db.GetIngredientByName(ingredientDishLink.Name);
            float newTotal = ing.TotalAmount - ingredientDishLink.Amount;
            await _db.UpdateIngredientAmounts(ingredientDishLink.IngredientId, newTotal);

            // Remove Item from the Ingredients list (otherwise it wont be removed from UI)
            Ingredients.Remove(ingredientDishLink);
        }

        public async Task<IngredientsModel> CheckForIngredient(string ingredient)
        {
            IngredientsModel i = await _db.GetIngredientByName(ingredient);
            return i;
        }

        public async Task EditIngredientEntry(DishIngredientsList editedIng)
        {
            // Get total of amount for this ingredient, then also edit the total amount in the ingredients table
            IngredientDishModel ingredientTooUpdate = new IngredientDishModel{
                Id = editedIng.Id,
                IngredientId = editedIng.IngredientId,
                DishId = editedIng.DishId,
                Amount = editedIng.Amount,
            };

            // Get ingredient, get current amount, get difference between current and new, add or subtract difference to the total amount
            IngredientDishModel ingDish = await _db.GetSingleIngredientForDish(editedIng.Id);
            float amountDifference = editedIng.Amount - ingDish.Amount;
            IngredientsModel ing = await _db.GetIngredientByName(editedIng.Name);
            float newTotal = ing.TotalAmount + amountDifference;
            await _db.UpdateIngredientAmounts(editedIng.IngredientId, newTotal); // Best to do calculations before in this function

            //Also need to automatially increase the total for this incredient
            await _db.UpdateIngredientDishLink(ingredientTooUpdate);

        }
        
        private async void UpdateIngredientTotal()
        {

        }
    }
}
