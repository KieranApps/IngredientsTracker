using IngredientsTracker.ViewModels;

namespace IngredientsTracker;

public partial class IngredientsList : ContentPage
{
	public IngredientsList(Data.Database db)
    {
        InitializeComponent();
        BindingContext = new IngredientsListVM(db);
    }
}