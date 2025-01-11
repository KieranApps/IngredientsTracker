using IngredientsTracker.ViewModels;

namespace IngredientsTracker;

public partial class Login : ContentPage
{
    
    public Login()
	{
		InitializeComponent();
        BindingContext = new LoginVM();
    }
}