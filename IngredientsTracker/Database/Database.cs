using IngredientsTracker.QueryClassDefs;
using SQLite;
using System.Xml.Linq;

namespace IngredientsTracker.Database
{
    public class Database
    {
        private readonly SQLiteAsyncConnection _database;

        public Database(string dbPath)
        {

            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<DishModel>().Wait(); // Copy this for any other model/table
            _database.CreateTableAsync<IngredientsModel>().Wait();
            _database.CreateTableAsync<IngredientDishModel>().Wait();
            _database.CreateTableAsync<ShoppingList>().Wait();
            _database.CreateTableAsync<Schedule>().Wait();
        }

        // Generic Save, should work for all saves of items
        public Task<int> SaveItemAsync<T>(T item) where T : new()
        {
            return _database.InsertAsync(item);
        }

        // Generic get all of a certain model
        public Task<List<T>> GetItemsAsync<T>() where T : new()
        {
            return _database.Table<T>().ToListAsync();
        }
       
        // Generic Delete item of any model (whole single item)
        public Task<int> DeleteItemAsync<T>(T item) where T : new ()
        {
            return _database.DeleteAsync(item);

        }

        // Get Specific Dish
        public Task<DishModel> GetDishModel(int id)
        {
            return _database.Table<DishModel>().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        // Get ingredients for a Dish
        public Task<List<DishIngredientsList>> GetIngredientsForDish(int dishId)
        {
            // Raw SQL needed to join tables for single query
            string sqlQuery = @" SELECT IngredientDishModel.Id, IngredientsModel.Name, IngredientDishModel.Amount,
                IngredientDishmodel.IngredientId, IngredientDishmodel.DishId FROM IngredientDishModel " +
                "JOIN IngredientsModel ON  IngredientDishModel.IngredientId = IngredientsModel.id " +
                "WHERE IngredientDishModel.DishId = ?";

            // Maybe need to change type of the function? Since it thinks it is an IngredientDishModel, which does not have name etc...
            return _database.QueryAsync<DishIngredientsList>(sqlQuery, dishId);
        }

        public Task<IngredientsModel> GetIngredientByName(string name)
        {
            return _database.Table<IngredientsModel>().Where(x => x.Name == name).FirstOrDefaultAsync();
        }

        public Task<int> RemoveDishIngLink(int dishId, int ingredientId)
        {
            return _database.ExecuteAsync("DELETE FROM IngredientDishModel WHERE DishId = ? AND IngredientId = ?", dishId, ingredientId);
        }

        public Task<int> UpdateIngredientDishLink(IngredientDishModel ingredientToUpdate)
        {
            return _database.UpdateAsync(ingredientToUpdate);
        }

        public Task<int> UpdateIngredientAmounts(int id, float amount)
        {
            return _database.ExecuteAsync("UPDATE IngredientsModel SET TotalAmount = ? WHERE Id = ?", amount, id);
        }

        public Task<IngredientDishModel> GetSingleIngredientForDish(int id)
        {
            return _database.Table<IngredientDishModel>().Where(x => x.Id == id).FirstOrDefaultAsync();
        }
    }
}
