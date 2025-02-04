using CommunityToolkit.Maui.Views;
using IngredientsTracker.Data;
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
        await vm.GetAllDishes();
        Label label = (Label)sender;
        var tapGesture = (TapGestureRecognizer)label.GestureRecognizers.FirstOrDefault();
        var day = tapGesture.CommandParameter as CalendarDay;
        var popup = new DishListModal(vm, day);
        await this.ShowPopupAsync(popup);
    }

    private void HideDishModal(object sender, FocusEventArgs e)
    {
        
    }
}