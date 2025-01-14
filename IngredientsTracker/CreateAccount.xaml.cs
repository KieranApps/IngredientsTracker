using IngredientsTracker.ViewModels;

namespace IngredientsTracker;

public partial class CreateAccount : ContentPage
{

    CreateAccountVM createAccountVM;

    public CreateAccount()
	{
		InitializeComponent();
        createAccountVM = App.ServiceProvider.GetService<CreateAccountVM>();
        BindingContext = createAccountVM;
    }

    public async void SubmitDetails(object sender, EventArgs e)
    {
        bool validPass = createAccountVM.PasswordValid();
        if (!validPass) {
            return;
        }
        bool validEmail = createAccountVM.EmailValid();
        if (!validEmail)
        {
            return;
        }

        // Submit the account details
        bool successfullyCreated = await createAccountVM.CreateAccountAndLogin();
        if (successfullyCreated)
        {
            var homePage = App.ServiceProvider.GetService<HomePage>();
            App.Current.MainPage = new NavigationPage(homePage);
        }
    }

    public void CheckEmailIsValid(object sender, EventArgs e)
    {
        bool validEmail = createAccountVM.EmailValid();
        if (!validEmail)
        {
            errorEmail.IsVisible = true;
        }
        else
        {
            errorEmail.IsVisible = false;
        }
    }

    public void CheckPasswordIsValid(object sender, EventArgs e)
    {
        bool validPass = createAccountVM.PasswordValid();
        if (!validPass)
        {
            errorPassword.IsVisible = true;
        }
        else
        {
            errorPassword.IsVisible = false;
        }
    }
}