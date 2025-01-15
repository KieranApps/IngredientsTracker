using IngredientsTracker.Helpers;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace IngredientsTracker.ViewModels
{
    public partial class CreateAccountVM : BindableObject
    {

        private readonly ApiService _api;

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

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

        public CreateAccountVM() { }
        public CreateAccountVM(ApiService api)
        {
            _api = api;
        }

        public bool PasswordValid()
        {
            // Check Password
            if (_password.Length < 8 || _password.Length > 100)
            {
                return false;
            }
            if (!Regex.IsMatch(_password, "[a-z]"))
            {
                return false;
            }
            if (!Regex.IsMatch(_password, "[A-Z]"))
            {
                return false;
            }
            if (!Regex.IsMatch(_password, "[0-9]"))
            {
                return false;
            }
            return true;
        }

        public bool EmailValid()
        {
            Regex emailRegex = new Regex("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+[.][a-z]{2,}$");
            if (!emailRegex.IsMatch(_email))
            {
                return false;
            }

            return true;
        }

        public async Task<bool> CreateAccountAndLogin()
        {
            // No need to check pass and email again already been done in code-behind
            string response = await _api.CreateAccount(_name, _email, _password);
            JObject responseData = JObject.Parse(response);
            bool success = (bool)responseData["success"];
            if (!success)
            {
                Debug.WriteLine("Error Creating Account");
                // Set error true, to show error text on input screen
                return false;
            }

            string logInResponse = await _api.Login(_email, _password);
            JObject loginResponseData = JObject.Parse(logInResponse);
            bool loginSuccess = (bool)loginResponseData["success"];
            if (!loginSuccess)
            {
                Debug.WriteLine("Error Logging in");
                return false;
            }
            string refreshToken = (string)loginResponseData["tokens"]["refreshToken"];
            string accessToken = (string)loginResponseData["tokens"]["accessToken"];
            string id = (string)loginResponseData["userInfo"]["id"];
            string email = (string)loginResponseData["userInfo"]["email"];
            string name = (string)loginResponseData["userInfo"]["name"];

            TokenHandler tokenHandler = new TokenHandler();
            UserService userService = new UserService();

            // Save User Info
            await userService.SaveUserId(id);
            await userService.SaveUserEmail(email);
            await userService.SaveUserName(name);

            await tokenHandler.SaveRefreshToken(refreshToken);
            await tokenHandler.SaveAccessToken(accessToken);
            return true;
        }
    }
}
