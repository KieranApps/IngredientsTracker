using System.Text;
using System.Reflection;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace IngredientsTracker.Helpers
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly TokenHandler _tokenHandler;

        string host;

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

        // Will only be called within this class
        private async Task<bool> AttemptTokenRefresh()
        {
            Uri uri = new Uri(host + "/auth/refresh");
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            string token = await _tokenHandler.GetRefreshToken();
            request.Headers.Add("token", token);
            var response = await _httpClient.SendAsync(request);
            // Refresh not successful so all tokens expired/invalid
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            string responseString = await response.Content.ReadAsStringAsync();
            JObject responseData = JObject.Parse(responseString);

            string refreshToken = (string)responseData["tokens"]["refreshToken"];
            string accessToken = (string)responseData["tokens"]["accessToken"];
            await _tokenHandler.SaveRefreshToken(refreshToken);
            await _tokenHandler.SaveAccessToken(accessToken);
            return true;
        }

        private async Task<HttpResponseMessage> RetryRequest(HttpRequestMessage request)
        {
            // Try and refresh, then retry the original request
            bool successfulRefresh = await AttemptTokenRefresh();
            if (!successfulRefresh)
            {
                // Log out
                _tokenHandler.DeleteRefreshToken();
                _tokenHandler.DeleteAccessToken();
                // Go to main page
                var homePage = App.ServiceProvider.GetService<MainPage>();
                App.Current.MainPage = new NavigationPage(homePage);

                return new HttpResponseMessage(); // return blank, so content is just null and will finish off whatever was running without action
            }

            request.Headers.Remove("token");
            string token = await _tokenHandler.GetAccessToken();
            request.Headers.Add("token", token);
            return await _httpClient.SendAsync(request);
        }
        
        // This function will only be used on start up, even if very similar to normal RefershTokens function. Just for ease of reading
        public async Task<bool> CheckTokensAreValidOnBoot(string refreshToken)
        {
            try
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
            catch (Exception ex) {
                return false;
            }
        }
        

        /**
         * --------------------------------
         * Main APIs below this point
         * --------------------------------
         */
        public async Task<string> Login(string email, string password)
        {
            try
            {
                Uri uri = new Uri(host + "/user/login");
                var request = new HttpRequestMessage(HttpMethod.Post, uri);
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
            catch (Exception ex) {
                return "{success: false}";
            }
        }

        public async Task<string> CreateAccount(string name, string email, string password)
        {
            try
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
            catch (Exception ex) {
                return "{success: false}";
            }
        }

        public async Task<string> AddNewDish(string dishName)
        {
            try
            {
                Uri uri = new Uri(host + "/dish/add");
                var request = new HttpRequestMessage(HttpMethod.Post, uri);
                string token = await _tokenHandler.GetAccessToken();
                request.Headers.Add("token", token);
                var body = new
                {
                    name = dishName
                };
                string payload = JsonSerializer.Serialize(body);
                request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode.ToString() == "Unauthorized")
                    {
                        response = await RetryRequest(request); 
                        // Response is blank if un auth and force log out, which is 200 code. Content is null
                        if (!response.IsSuccessStatusCode)
                        {
                            // Something else went wrong with the server that is not Auth related.
                            return "{success: false}";
                        }
                    }
                    else
                    {
                        return "{success: false}";
                    }
                }
                string responseData = await response.Content.ReadAsStringAsync();
                return responseData;
            }
            catch (Exception ex) {
                return "{success: false}";
            }
        }
    }
}
