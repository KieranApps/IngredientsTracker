using IngredientsTracker.ViewModels;

namespace IngredientsTracker;

public partial class Login : ContentPage
{
    
    public Login()
	{
		InitializeComponent();
        var loginVM = App.ServiceProvider.GetService<LoginVM>();
        BindingContext = loginVM;
    }
}