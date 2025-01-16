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
    
}