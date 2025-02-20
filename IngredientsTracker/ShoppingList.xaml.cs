using IngredientsTracker.ViewModels;

namespace IngredientsTracker;

public partial class ShoppingList : ContentPage
{

    public ShoppingListVM vm;

	public ShoppingList()
	{
		InitializeComponent();
        vm = App.ServiceProvider.GetService<ShoppingListVM>();
        BindingContext = vm;
    }
}