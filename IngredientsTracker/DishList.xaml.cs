using IngredientsTracker.Database;
using IngredientsTracker.ViewModels;

namespace IngredientsTracker;

public partial class DishList : ContentPage
{
    DishListVM vm;

    public DishList()
	{
        InitializeComponent();
        vm = App.ServiceProvider.GetService<DishListVM>();
        BindingContext = vm;
    }

    public async void SubmitNewDishClick(object sender, EventArgs e)
    {
       await vm.AddDish();
    }

    public async void GoToDish(object sender, EventArgs e)
    {
        var frame = (Frame)sender;
        var tapGesture = (TapGestureRecognizer)frame.GestureRecognizers.FirstOrDefault();
        var dish = tapGesture.CommandParameter as DishModel;
        var dishInfo = App.ServiceProvider.GetService<DishInformation>();
        dishInfo.SetDish(dish);
        Navigation.PushAsync(dishInfo);
    }
    
}