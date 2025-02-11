using IngredientsTracker.Data;
using IngredientsTracker.Helpers;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace IngredientsTracker.ViewModels
{
    public partial class StockListVM : BindableObject
    {
        private readonly ApiService _api;

        public ObservableCollection<StockItem> StockItems { get; set; }
        public ObservableCollection<IngredientSearchResult> SearchResults { get; set; }
        public event EventHandler<IEnumerable<IngredientSearchResult>> SearchResultsReady;
        
        private JArray AllUnits { get; set; }
        public JObject UnitMapping { get; set; }

        public bool optionSelected = false;

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
                _newIngredientAmount = value;
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


        public StockListVM() { }
        public StockListVM(ApiService api)
        {
            _api = api;
            StockItems = new ObservableCollection<StockItem>();
            SearchResults = new ObservableCollection<IngredientSearchResult>();
            AllUnits = new JArray();
            UnitMapping = new JObject();
            LoadUnits();
            LoadStock();
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
            Units = new ObservableCollection<string>();
            AllUnits = (JArray)responseData["units"];
            UnitMapping = (JObject)responseData["unitMapping"];
            // Assign units to variable for Binding
            foreach (JObject entry in responseData["units"])
            {
                Units.Add((string)entry["unit"]);
            }
        }
        
        private async Task LoadStock()
        {
            string response = await _api.GetStock();
            JObject responseData = JObject.Parse(response);
            bool success = (bool)responseData["success"];
            if (!success)
            {
                // error message
                return;
            }

            JArray stock = (JArray)responseData["stock"]; // Save so we can get the ID for the unit when submitting

            // Assign units to variable for Binding
            foreach (JObject item in stock)
            {
                string unitName = string.Empty;
                foreach (var unit in AllUnits)
                {
                    if ((string)unit["id"] == (string)item["unit_id"])
                    {
                        unitName = (string)unit["unit"];
                    }
                }

                StockItems.Add(new StockItem
                {
                    Id = (int)item["id"],
                    UserId = (int)item["user_id"],
                    IngredientId = (int)item["ingredient_id"],
                    Ingredient = (string)item["ingredient_name"],
                    Amount = (string)item["amount"],
                    UnitId = (int)item["unit_id"],
                    Unit = (string)item["unit"]
                });
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

            if (string.IsNullOrEmpty(NewIngredient.Trim()) || NewIngredient.Length < 2)
            {
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
                    Id = (int)entry["id"],
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
            int unitId = -1;
            string unitName = string.Empty;
            foreach (var unit in AllUnits)
            {
                if ((string)unit["unit"] == ChosenUnit)
                {
                    unitId = (int)unit["id"];
                    unitName = (string)unit["unit"];
                }
            }

            if (!float.TryParse(NewIngredientAmount, out _))
            {
                return false; // Amount must be number
            }
            var response = await _api.SubmitStockIngredient(NewSelectedIngredient.Id, NewIngredientAmount, unitId);
            JObject responseData = JObject.Parse(response);
            bool success = (bool)responseData["success"];
            if (!success)
            {
                return false;
            }
            UserService us = new UserService();
            string userId = await us.getUserId();
            int user_id = int.Parse(userId);
            // Add to IngredientList and then remove form new params
            StockItems.Add(new StockItem
            {
                Id = (int)responseData["result"],
                UserId = user_id,
                Amount = NewIngredientAmount,
                UnitId = unitId,
                Unit = unitName
            });

            NewIngredient = "";
            NewIngredientAmount = "";
            ChosenUnit = "";
            SearchResults.Clear();
            return true;
        }
    }
}
