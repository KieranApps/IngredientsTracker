using IngredientsTracker.Database;
using IngredientsTracker.ViewModels;
using System.Diagnostics;

namespace IngredientsTracker;

public partial class DishInformation : ContentPage
{
    DishInformationVM vm;
    DishModel _dish;

    private Timer _timer;
    private const int pauseTime = 500; // ms
                                       // 
	public DishInformation()
    {
        InitializeComponent();
        vm = App.ServiceProvider.GetService<DishInformationVM>();
        BindingContext = vm;
    }

    public void SetDish(DishModel dish)
    {
        _dish = dish;
        vm.SetDish(dish);

        DishInfoTitle.Text = dish.Name + " Information";
    }

    private void OnIngredientEntryTextChanged(object sender, EventArgs e)
    {
        _timer?.Dispose();

        // Create a new timer that triggers after the user pauses typing for 1 second
        _timer = new Timer(async _ =>
        {
            // The user paused typing, so do your API call
            await vm.SearchIngredients(); // No need for param, we can use NewIngredient in the VM

        }, null, pauseTime, Timeout.Infinite);
    }

    public void UpdateChosenUnit(object sender, EventArgs e)
    {
        Debug.WriteLine("Perform update");
    }
}