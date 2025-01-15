using System.Text;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace IngredientsTracker.Helpers
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly TokenHandler _tokenHandler;
        string host;


        /**
         * function foo(...params)
         * call api
         * if 401
         *     Do token auth checks, log out if needed (i.e., the refresh is bad too)
         *     if tokenCheck == 401: log out (delete tokens and navigate to main page via function in code-behind and remove all history
         *     // Nav to main page and delete all history like this:
         *         var mainPage = App.ServiceProvider.GetService<MainPage>();
         *         App.Current.MainPage = new NavigationPage(mainPage); // i.e., we create a NEW navigation page and stack/history
         *         // ^^ This is the SAME as is in Login.xaml.cs
         *     else continue below...
         *     Save tokens
         *     // Will only retry if tokens HAVE been refreshed and saved. So there should be NO auth issue and 401
         *     // if refersh is also un auth, we check that in here, and if IS, then log out
         *     // So both tokens are checked. First access then refresh. If both un auth, log out.
         *     // i.e., if access un auth, try refresh, if refresh un auth, log out
         *     call same api and re assign the response (since not a const
         * The recursive bit will be in the VMs
         */

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _tokenHandler = new TokenHandler();

            var assembly = Assembly.GetExecutingAssembly();
            using Stream stream = assembly.GetManifestResourceStream("IngredientsTracker.Resources.config.json");
            using StreamReader reader = new StreamReader(stream);
            string json = reader.ReadToEnd();

            var configData = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            host = configData["ApiHost"];
        }

                // This function will only be used on start up, even if very similar to normal RefershTokens function. Just for ease of reading
        public async Task<bool> CheckTokensAreValidOnBoot(string refreshToken)
        {
            Uri uri = new Uri(host + "/auth/refresh");
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add("token", refreshToken);
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            
            // If success, i.e., still authorised, save tokens to be updated
            string responseString = await response.Content.ReadAsStringAsync();
            JObject responseData = JObject.Parse(responseString);

            string newRefreshToken = (string)responseData["tokens"]["refreshToken"];
            string accessToken = (string)responseData["tokens"]["accessToken"];
            await _tokenHandler.SaveRefreshToken(newRefreshToken);
            await _tokenHandler.SaveAccessToken(accessToken);
            return true;
        }

        public async Task<bool> RefreshTokens()
        {
            string refreshToken = await _tokenHandler.GetRefreshToken();

            return true;
        }

        public async Task<string> Login(string email, string password)
        {
            Uri uri = new Uri(host + "/user/login");
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshToken); // Headers + tokens not needed for login
            var body = new
            {
                email = email,
                password = password
            };
            string payload = JsonSerializer.Serialize(body);
            request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) 
            {
                return "{success: false}";
            }
            string responseData = await response.Content.ReadAsStringAsync();
            return responseData;
        }

        public async Task<string> CreateAccount(string name, string email, string password)
        {
            Uri uri = new Uri(host + "/user/create-user");
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            var body = new
            {
                name = name,
                email = email,
                password = password
            };
            string payload = JsonSerializer.Serialize(body);
            request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return "{success: false}";
            }

            string responseData = await response.Content.ReadAsStringAsync();
            return responseData;
        }
    }
}
