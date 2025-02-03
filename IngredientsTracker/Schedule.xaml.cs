using CommunityToolkit.Maui.Views;
using IngredientsTracker.Modals;
using IngredientsTracker.ViewModels;
using System.Diagnostics;

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
        var popup = new DishListModal(vm);
        await this.ShowPopupAsync(popup);

    }

    private async void AssignDishToDay(object sender, TappedEventArgs e)
    {
        Debug.WriteLine("Assigining");
        
    }

    private void HideDishModal(object sender, FocusEventArgs e)
    {
        
    }
}