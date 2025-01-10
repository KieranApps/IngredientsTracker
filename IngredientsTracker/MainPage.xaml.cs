using IngredientsTracker.Database;
using IngredientsTracker.Helpers;
using Microsoft.Extensions.Logging;

namespace IngredientsTracker
{
    public partial class MainPage : ContentPage
    {

        private readonly ApiService _api;

        public MainPage(ApiService api)
        {
            InitializeComponent();
            _api = api;

            LoadTokensAndCheckSession();
        }

        public async void LoadTokensAndCheckSession()
        {
            TokenHandler tokenHandler = new TokenHandler();
            string refreshToken = await tokenHandler.GetRefreshToken();
            if (refreshToken == null) // We dont have refresh token so login
            {
                // Delete all tokens to ensure all is empty
                tokenHandler.DeleteRefreshToken();
                tokenHandler.DeleteAccessToken();
                return;
            }
            // Check refresh token is valid
            bool isValid = await _api.CheckTokensAreValid(refreshToken);

            if (isValid)
            {
                //Skip the login screen
                await Navigation.PushAsync(new HomePage()); // Edit for DI
            }
        }

        private void ViewCreateAccount(object sender, EventArgs e)
        {
            var createAccount = App.ServiceProvider.GetService<CreateAccount>();
            Navigation.PushAsync(createAccount);
        }

        private void ViewLogin(object sender, EventArgs e)
        {
            var login = App.ServiceProvider.GetService<Login>();
            Navigation.PushAsync(login);
        }
    }

}
