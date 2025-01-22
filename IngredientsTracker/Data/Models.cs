using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngredientsTracker.Database
{
    public class DishModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public int User_Id { get; set; }
    }

    // This will track all ingredients across all dishes
    public class IngredientsModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public float Amount { get; set; } // Current item count (i.e., 2.5 onions)
        public float TotalAmount {  get; set; } // For the amount bought/have at the start of the week (i.e., 5 onions)
    }

    public class DishIngredientsList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Amount { get; set; }
        public int DishId { get; set; }
        public int IngredientId { get; set; }
    }

    public class IngredientSearchResults
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    // Table to track ingredients and amounts to each dish
    public class IngredientDishModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int DishId { get; set; }
        public int IngredientId { get; set; }
        public float Amount { get; set; } // Amount used in the linked dish

    }

    public class ShoppingList
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } // May not really need the ID, but good to keep in DB anyway
        public string Name { get; set; }
        public float Count { get; set; } // Take this from the items total count to add to the list
    }

    public class Schedule
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int DishId { get; set; }
        public bool Complete { get; set; }

    }
}
