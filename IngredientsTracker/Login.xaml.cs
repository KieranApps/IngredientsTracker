using IngredientsTracker.ViewModels;

namespace IngredientsTracker;

public partial class Login : ContentPage
{
    LoginVM loginVM;
    public Login()
	{
		InitializeComponent();
        loginVM = App.ServiceProvider.GetService<LoginVM>();
        BindingContext = loginVM;
    }

    public async void HandleLoginClick(object sender, EventArgs e)
    {
        bool loggedIn = await loginVM.Login();

        if (loggedIn)
        {
            var homePage = App.ServiceProvider.GetService<HomePage>();
            App.Current.MainPage = new NavigationPage(homePage);
        }
        else {
            // Show error
            errorMessage.IsVisible = true;
        }
    }
}