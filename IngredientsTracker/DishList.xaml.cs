using IngredientsTracker.Database;
using IngredientsTracker.ViewModels;
using System.Collections.ObjectModel;

namespace IngredientsTracker;

public partial class DishList : ContentPage
{
    
    public DishList(Database.Database db)
	{
        InitializeComponent();
        BindingContext = new DishListVM(db);
    }
}