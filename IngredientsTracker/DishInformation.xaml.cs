using IngredientsTracker.Database;
using IngredientsTracker.ViewModels;

namespace IngredientsTracker;

public partial class DishInformation : ContentPage
{
    DishInformationVM vm;
    DishModel _dish;
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
}