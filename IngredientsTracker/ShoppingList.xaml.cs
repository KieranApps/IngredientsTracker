using IngredientsTracker.Data;
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
        vm.AddNewItemToList();
    }

    private async void AddNewItem(object sender, FocusEventArgs e)
    {
        Entry entry = (Entry)sender;
        ShoppingListItem item = entry.ReturnCommandParameter as ShoppingListItem;
        await vm.AddNewItem(item);
        // Check binding for ID (ignore and return if ID exists)
    }

    private async void DeleteItem(object sender, FocusEventArgs e)
    {
        await vm.DeleteItem();
    }
}