using IngredientsTracker.Data;
using IngredientsTracker.Helpers;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace IngredientsTracker.ViewModels
{
    public partial class DishInformationVM : BindableObject 
    {
        private readonly ApiService _api;
        public ObservableCollection<DishIngredientsList> Ingredients { get; set; }
        public ObservableCollection<IngredientSearchResult> SearchResults { get; set; }
        public event EventHandler<IEnumerable<IngredientSearchResult>> SearchResultsReady;

        public bool optionSelected = false;

        public DishModel CurrentDish;

        public JArray unitNameIdMap;

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

        public IngredientSearchResult NewSelectedIngredient { get; set; }

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

        private ObservableCollection<string> _units;
        public ObservableCollection<string> Units
        {
            get => _units;
            set
            {
                _units = value;
                OnPropertyChanged(nameof(Units));
            }
        }

        private string _chosenUnit;
        public string ChosenUnit
        {
            get => _chosenUnit;
            set
            {
                _chosenUnit = value;
                OnPropertyChanged();
            }
        }

        public DishInformationVM() { }
        public DishInformationVM(ApiService api)
        {
            _api = api;
            Ingredients = new ObservableCollection<DishIngredientsList>();
            SearchResults = new ObservableCollection<IngredientSearchResult>();

            LoadIngredients();
            LoadUnits();
        }

        public void SetDish(DishModel dish)
        {
            CurrentDish = dish;
        }

        public async Task LoadIngredients()
        {

            
        }

        private async Task LoadUnits()
        {
            string response = await _api.GetAllUnits();

            JObject responseData = JObject.Parse(response);
            bool success = (bool)responseData["success"];
            if (!success)
            {
                // error message
                return;
            }
            string unitsAsString = responseData["units"].ToString(); // Save so we can get the ID for the unit when submitting
            unitNameIdMap = JArray.Parse(unitsAsString);

            Units = new ObservableCollection<string>();
            // Assign units to variable for Binding
            foreach (JObject entry in responseData["units"])
            {
                Units.Add((string)entry["unit"]);
            }
        }

        public async Task SearchIngredients()
        {
            if (optionSelected)
            {
                optionSelected = false;
                return;
            }
            SearchResults.Clear(); // Ensure empty

            if (string.IsNullOrEmpty(NewIngredient.Trim()) || NewIngredient.Length < 2) {
                return; // If we need to return something for pop up to behave just return bool. True to show, False to hide
            }

            string response = await _api.SearchIngredients(NewIngredient);
            JObject responseData = JObject.Parse(response);
            bool success = (bool)responseData["success"];
            if (!success)
            {
                // error message
                return;
            }

            foreach (var entry in responseData["results"])
            {
                SearchResults.Add(new IngredientSearchResult
                {
                    Id = (string)entry["id"],
                    Name = (string)entry["name"]
                });
            }

            if (SearchResults.Any())
            {
                SearchResultsReady?.Invoke(this, SearchResults);
            }
        }

        public async Task<bool> SubmitNewIngredient()
        {
            if (
                NewSelectedIngredient == null ||
                string.IsNullOrEmpty(NewIngredientAmount) ||
                string.IsNullOrEmpty(ChosenUnit)
                )
            {
                return false;
            }

            // Get ChosenUnit ID
            string unitId = "";
            foreach (var unit in unitNameIdMap)
            {
                if ((string)unit["unit"] == ChosenUnit)
                {
                    unitId = (string)unit["id"];
                }
            }

            if (!float.TryParse(NewIngredientAmount, out _))
            {
                return false; // Amount must be number
            }
            var response = await _api.SubmitIngredient(CurrentDish.Id, NewSelectedIngredient.Id, NewIngredientAmount, unitId);
            JObject responseData = JObject.Parse(response);
            bool success = (bool)responseData["success"];
            if (!success)
            {
                return false;
            }
            // Add to IngredientList and then remove form new params
            Ingredients.Add(new DishIngredientsList
            {
                Id = (int)responseData["result"],
                DishId = CurrentDish.Id,
                IngredientId = NewSelectedIngredient.Id,
                Amount = NewIngredientAmount,
                UnitId = unitId,
            });

            NewIngredient = "";
            NewIngredientAmount = "";
            ChosenUnit = "";
            SearchResults.Clear();
            
            return true;
        }
    }
}
