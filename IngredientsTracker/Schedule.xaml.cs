using IngredientsTracker.ViewModels;
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

}