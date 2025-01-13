using IngredientsTracker.ViewModels;
using System.Diagnostics;

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
            var mainPage = App.ServiceProvider.GetService<MainPage>();
            await Navigation.PushAsync(homePage);
            Navigation.RemovePage(mainPage);
            Navigation.RemovePage(this);
        }
        else {
            // Show error
        }
    }
}