using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace IngredientsTracker.Data
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
        public int DishId { get; set; }
        public int IngredientId { get; set; }
        public string Amount { get; set; }
        public string UnitId { get; set; }
        public string IngredientName { get; set; }
        public string UnitName { get; set; }
    }

    public class IngredientSearchResult
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

    public class CalendarDay : INotifyPropertyChanged
    {
        public string Date { get; set; }
        public string DateNumber { get; set; }
        public string DateDay { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _dishId;
        public int DishId
        {
            get => _dishId;
            set
            {
                if (_dishId != value)
                {
                    _dishId = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _completed;
        public bool Completed
        {
            get => _completed;
            set
            {
                if (_completed != value)
                {
                    _completed = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class StockItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int IngredientId { get; set; }
        public float Amount { get; set; }
        public int UnitId { get; set; }
        public string Unit { get; set; }
    }
}
