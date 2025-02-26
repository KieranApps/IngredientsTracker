using IngredientsTracker.Data;
using IngredientsTracker.Helpers;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;

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
                    IngredientId = (int?)item["ingredient_id"],
                    Amount = (string)item["amount"],
                    UnitId = (int?)item["unit_id"],
                    Unit = (string)item["unit"]
                });
            }
        }

        public void AddNewItemToList()
        {
            Items.Add(new ShoppingListItem());
        }

        public async Task AddNewItem(ShoppingListItem item)
        {
            // if (!item.Id) Add new else edit
            if (item.Id > 0)
            {
                return;
            }
            var itemsExist = Items.Where(x => x.Item == item.Item);
            if (itemsExist.Count() > 1)
            {
                return; // too many, can only have unique item names
            }
            // Check if an item with the same item name already exists
            string response = await _api.AddNewItemToShoppingList(item);
            JObject responseData = JObject.Parse(response);
            bool success = (bool)responseData["success"];
            if (!success)
            {
                // error message
                return;
            }
            // Update the collection
            ShoppingListItem? itemToUpdate = Items.FirstOrDefault(x => x.Item == item.Item); // We know it is unique here because of check above
            itemToUpdate.Id = (int)responseData["result"];
        }

        public async Task UpdateItem(ShoppingListItem item)
        {
            if (String.IsNullOrEmpty(item.Item))
            {
                return;
            }
            string response = await _api.EditSHoppingListItem(item);
            JObject responseData = JObject.Parse(response);
            bool success = (bool)responseData["success"];
            if (!success)
            {
                // error message
                return;
            }
        }

        public async Task DeleteItem()
        {

        }
    }
}
