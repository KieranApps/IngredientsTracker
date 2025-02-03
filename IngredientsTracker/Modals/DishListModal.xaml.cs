using CommunityToolkit.Maui.Views;
using IngredientsTracker.ViewModels;

namespace IngredientsTracker.Modals;

public partial class DishListModal : Popup
{
	public DishListModal(ScheduleVM vm)
	{
		InitializeComponent();
        BindingContext = vm;
	}
    private void OnCloseClicked(object sender, EventArgs e)
    {
        Close();
    }
}