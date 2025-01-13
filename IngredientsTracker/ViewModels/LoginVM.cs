using IngredientsTracker.Helpers;
using System.Diagnostics;
using System.Text.Json;
using System.Windows.Input;
using Newtonsoft.Json.Linq;

namespace IngredientsTracker.ViewModels
{
    public partial class LoginVM : BindableObject
    {
        
        private readonly ApiService _api;

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public LoginVM() { }
        public LoginVM(ApiService api) 
        {
            _api = api;
        }

        public ICommand LoginCommand => new Command(async () => await Login());


        public async Task<bool> Login()
        {
            Debug.WriteLine("TESTING");
            if (_email == null) { }
            if (_password == null) { }
            // Update to add some email validation
            string response = await _api.Login(_email, _password);
            JObject responseData = JObject.Parse(response);
            bool success = (bool) responseData["success"];
            if (!success)
            {
                Debug.WriteLine("Error Logging in");
                // Set error true, to show error text on input screen
                return false;
            }
            string refreshToken = (string)responseData["tokens"]["refreshToken"];
            string accessToken = (string)responseData["tokens"]["accessToken"];
            TokenHandler tokenHandler = new TokenHandler();
            // Usually token handler is not created in VMs, it is already in ApiService which is where 99% of its use will be
            // i.e., reading token from memory for API calls and occasional updated token saves.
            // But with login as its a slightly Unique case where things may not be able to be saved, we shall check and save here
            // Hence instantiating a TokenHandler Object.
            await tokenHandler.SaveRefreshToken(refreshToken);
            await tokenHandler.SaveAccessToken(accessToken);
            var homePage = App.ServiceProvider.GetService<HomePage>();
            return true;
        }
    }   
}
