using IngredientsTracker.ViewModels;
using System.Diagnostics;

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

    private void OnAddNewTapped(object sender, TappedEventArgs e)
    {
        Debug.WriteLine("On tap");
        vm.AddNewItemToList();
    }

    private void AddNewItem(object sender, FocusEventArgs e)
    {
        Debug.WriteLine("test");
        // Check binding for ID (ignore and return if ID exists)
    }
}