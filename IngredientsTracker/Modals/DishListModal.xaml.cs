using CommunityToolkit.Maui.Views;
using IngredientsTracker.Data;
using IngredientsTracker.ViewModels;

namespace IngredientsTracker.Modals;

public partial class DishListModal : Popup
{
    ScheduleVM _vm;
    CalendarDay _day;
	public DishListModal(ScheduleVM vm, CalendarDay day)
	{
		InitializeComponent();
        _vm = vm;
        _day = day;
        BindingContext = _vm;
	}

    private void OnCloseClicked(object sender, EventArgs e)
    {
        Close();
    }

    private async void OnDishTapped(object sender, TappedEventArgs e)
    {
        Label label = (Label)sender;
        var tapGesture = (TapGestureRecognizer)label.GestureRecognizers.FirstOrDefault();
        var dish = tapGesture.CommandParameter as DishModel;
        bool success = await _vm.AddDishToSchedule(dish, _day);
        if (success)
        {
            Close();
        }
    }
}