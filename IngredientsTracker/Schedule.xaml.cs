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

    public void OpenAssignDishModal()
    {

    }

    private void GoToDish(object sender, TappedEventArgs e)
    {
        Debug.WriteLine("Going to dish");
    }
}