using IngredientsTracker.ViewModels;
using System.Diagnostics;
using static IngredientsTracker.ViewModels.ScheduleVM;

namespace IngredientsTracker;

public partial class Schedule : ContentPage
{
    private ScheduleVM vm;
    public Schedule()
	{
		InitializeComponent();
        vm = App.ServiceProvider.GetService<ScheduleVM>();
        BindingContext = vm; ;
    }

    private void GoToDish(object sender, TappedEventArgs e)
    {
        Debug.WriteLine("Going to dish");
    }

    private async void OpenAssignDishModal(object sender, TappedEventArgs e)
    {
        // Get All dishes
        Debug.WriteLine("OpenModel");
        await vm.GetAllDishes();
        DishesModal.IsVisible = true;

    }

    private async void AssignDishToDay(object sender, TappedEventArgs e)
    {
        Debug.WriteLine("Assigining");
        DishesModal.IsVisible = false;
    }

    private void HideDishModal(object sender, FocusEventArgs e)
    {
        DishesModal.IsVisible = false;
    }
}