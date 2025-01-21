using IngredientsTracker.Database;
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
            Units = new ObservableCollection<string>();
            // Assign units to variable for Binding
            foreach (JObject entry in responseData["units"])
            {
                Units.Add((string)entry["unit"]);
            }
        }
    }
}
