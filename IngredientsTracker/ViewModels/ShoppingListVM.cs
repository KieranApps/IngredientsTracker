using IngredientsTracker.Data;
using IngredientsTracker.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngredientsTracker.ViewModels
{
    public partial class ShoppingListVM : BindableObject
    {

        private readonly ApiService _api;
        public ObservableCollection<ShoppingListItem> Items { get; set;  }

        public ShoppingListVM() { }
        public ShoppingListVM(ApiService api)
        {
            _api = api;
            Items = new ObservableCollection<ShoppingListItem>();
            
            GetShoppingList();
        }

        public async Task GetShoppingList()
        { 
            string response = await _api.GetShoppingList();
            JObject responseData = JObject.Parse(response);
            bool success = (bool)responseData["success"];
            if (!success)
            {
                // error message
                return;
            }

            JArray shoppingList = (JArray)responseData["list"]; // Save so we can get the ID for the unit when submitting

            // Assign units to variable for Binding
            foreach (JObject item in shoppingList)
            {
                Items.Add(new ShoppingListItem
                {
                    Id = (int)item["id"],
                    UserId = (int)item["user_id"],
                    Item = (string)item["item"],
                    IngredientId = (int)item["ingredient_id"],
                    Amount = (string)item["amount"],
                    UnitId = (int)item["unit_id"],
                    Unit = (string)item["unit"]
                });
            }
        }

        public void AddNewItemToList()
        {
            Items.Add(new ShoppingListItem());
        }

        public async Task <bool> UpdateItem()
        {

            // if (!item.Id) Add new else edit
            return false;
        }
    }
}
