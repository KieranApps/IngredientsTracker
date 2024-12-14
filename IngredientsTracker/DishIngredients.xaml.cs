using IngredientsTracker.Database;
using IngredientsTracker.QueryClassDefs;
using IngredientsTracker.ViewModels;

namespace IngredientsTracker;

public partial class DishIngredients : ContentPage
{
    DishIngredientsVM vm;
	public DishIngredients(Database.Database db, DishModel dish)
    {
        InitializeComponent();
        vm = new DishIngredientsVM(db, dish);
        BindingContext = vm;
    }

    public async void EditAmount(object sender, FocusEventArgs e)
    {
        if (sender is Entry entry)
        {
            DishIngredientsList editedIng = (DishIngredientsList) entry.ReturnCommandParameter;
            await vm.EditIngredientEntry(editedIng);
        }
    }
}