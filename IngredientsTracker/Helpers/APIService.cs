using System.Text;
using System.Reflection;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using IngredientsTracker.Data;

namespace IngredientsTracker.Helpers
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly TokenHandler _tokenHandler;
        private readonly UserService _userService;

        string host;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _tokenHandler = new TokenHandler();
            _userService = new UserService();

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

        /**
         *  This request param must be a FRESH request, or it wont be send a second time.
         *  It must be built before being passed as a param, so this fuction is universal for ALL request types with/without body content etc...
         *  
         *  Could be worth trying to create two of these (overrides) so that there is a post and get equivalent function for retry, neatens up each API call
         */
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

            string token = await _tokenHandler.GetAccessToken();
            request.Headers.Add("token", token);

            var response = await _httpClient.SendAsync(request);

            return response;
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

                string id = await _userService.getUserId();
                var body = new
                {
                    user_id = id,
                    name = dishName
                };
                string payload = JsonSerializer.Serialize(body);

                request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode.ToString() == "Unauthorized")
                    {
                        var freshRequest = new HttpRequestMessage(HttpMethod.Post, uri);
                        // No need to add token, as it should get a new one
                        freshRequest.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                        response = await RetryRequest(freshRequest); 
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

        public async Task<string> GetAllDishes()
        {
            try
            {
                string userId = await _userService.getUserId();
            
                Uri uri = new Uri(host + "/dish/all/" + userId);
                var request = new HttpRequestMessage(HttpMethod.Get, uri);

                string token = await _tokenHandler.GetAccessToken();
                request.Headers.Add("token", token);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode.ToString() == "Unauthorized")
                    {
                        var freshRequest = new HttpRequestMessage(HttpMethod.Get, uri);
                        // No need to add token, as it should get a new one
                        response = await RetryRequest(freshRequest);
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

        public async Task<string> GetAllUnits()
        {
            try
            {
                Uri uri = new Uri(host + "/ingredients/units");
                var request = new HttpRequestMessage(HttpMethod.Get, uri);

                string token = await _tokenHandler.GetAccessToken();
                request.Headers.Add("token", token);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode.ToString() == "Unauthorized")
                    {
                        var freshRequest = new HttpRequestMessage(HttpMethod.Get, uri);
                        response = await RetryRequest(freshRequest);
                        if (!response.IsSuccessStatusCode)
                        {
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
            catch (Exception ex)
            {
                return "{success: false}";
            }
        }

        public async Task<string> SearchIngredients(string term)
        {
            try
            {
                string user_id = await _userService.getUserId();
                Uri uri = new Uri(host + "/ingredients/search/" + user_id + "/" + term);
                var request = new HttpRequestMessage(HttpMethod.Get, uri);

                string token = await _tokenHandler.GetAccessToken();
                request.Headers.Add("token", token);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode.ToString() == "Unauthorized")
                    {
                        var freshRequest = new HttpRequestMessage(HttpMethod.Get, uri);
                        response = await RetryRequest(freshRequest);
                        if (!response.IsSuccessStatusCode)
                        {
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
            catch (Exception ex)
            {
                return "{success: false}";
            }
        }

        public async Task<string> SubmitIngredient(int dish_id, int ingredient_id, string amount, string unit_id)
        {
            try
            {
                Uri uri = new Uri(host + "/dish/add/ingredient");
                var request = new HttpRequestMessage(HttpMethod.Post, uri);

                string token = await _tokenHandler.GetAccessToken();
                request.Headers.Add("token", token);

                string user_id = await _userService.getUserId();

                var body = new
                {
                    user_id,
                    dish_id,
                    ingredient_id,
                    amount,
                    unit_id
                };
                string payload = JsonSerializer.Serialize(body);

                request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode.ToString() == "Unauthorized")
                    {
                        var freshRequest = new HttpRequestMessage(HttpMethod.Post, uri);
                        freshRequest.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                        response = await RetryRequest(freshRequest);
                        if (!response.IsSuccessStatusCode)
                        {
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return "{success: false}";
            }
        }

        public async Task<string> SubmitStockIngredient(int ingredient_id, string amount, int unit_id)
        {
            try
            {
                Uri uri = new Uri(host + "/stock/add");
                var request = new HttpRequestMessage(HttpMethod.Post, uri);

                string token = await _tokenHandler.GetAccessToken();
                request.Headers.Add("token", token);

                string user_id = await _userService.getUserId();

                var body = new
                {
                    user_id,
                    ingredient_id,
                    amount,
                    unit_id
                };
                string payload = JsonSerializer.Serialize(body);

                request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode.ToString() == "Unauthorized")
                    {
                        var freshRequest = new HttpRequestMessage(HttpMethod.Post, uri);
                        freshRequest.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                        response = await RetryRequest(freshRequest);
                        if (!response.IsSuccessStatusCode)
                        {
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return "{success: false}";
            }
        }

        public async Task<string> GetIngredientsForDish(int dish_id)
        {
            try
            {
                Uri uri = new Uri(host + "/ingredients/all/" + dish_id);
                var request = new HttpRequestMessage(HttpMethod.Get, uri);

                string token = await _tokenHandler.GetAccessToken();
                request.Headers.Add("token", token);

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode.ToString() == "Unauthorized")
                    {
                        var freshRequest = new HttpRequestMessage(HttpMethod.Get, uri);
                        response = await RetryRequest(freshRequest);
                        if (!response.IsSuccessStatusCode)
                        {
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
            catch (Exception ex)
            {
                return "{success: false}";
            }
        }

        public async Task<string> GetSchedule(DateTime startDate, DateTime endDate)
        {
            try
            {
                string user_id = await _userService.getUserId();
                Uri uri = new Uri(host + "/schedule/" + user_id + "/" + startDate.ToString("yyyy-MM-dd") + "/" + endDate.ToString("yyyy-MM-dd"));
                var request = new HttpRequestMessage(HttpMethod.Get, uri);

                string token = await _tokenHandler.GetAccessToken();
                request.Headers.Add("token", token);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode.ToString() == "Unauthorized")
                    {
                        var freshRequest = new HttpRequestMessage(HttpMethod.Get, uri);
                        response = await RetryRequest(freshRequest);
                        if (!response.IsSuccessStatusCode)
                        {
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
            catch (Exception ex)
            {
                return "{success: false}";
            }
        }

        public async Task<string> AddDishToSchedule(int dish_id, string date)
        {
            try
            {
                string user_id = await _userService.getUserId();
                Uri uri = new Uri(host + "/schedule/add");
                var request = new HttpRequestMessage(HttpMethod.Post, uri);

                string token = await _tokenHandler.GetAccessToken();
                request.Headers.Add("token", token);

                var body = new
                {
                    user_id,
                    dish_id,
                    date
                };
                string payload = JsonSerializer.Serialize(body);

                request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode.ToString() == "Unauthorized")
                    {
                        var freshRequest = new HttpRequestMessage(HttpMethod.Post, uri);
                        freshRequest.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                        response = await RetryRequest(freshRequest);
                        if (!response.IsSuccessStatusCode)
                        {
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
            catch (Exception ex)
            {
                return "{success: false}";
            }
        }

        public async Task<string> EditDishOnSchedule(int dish_id, string date)
        {
            try
            {
                string user_id = await _userService.getUserId();
                Uri uri = new Uri(host + "/schedule/edit");
                var request = new HttpRequestMessage(HttpMethod.Post, uri);

                string token = await _tokenHandler.GetAccessToken();
                request.Headers.Add("token", token);

                var body = new
                {
                    user_id,
                    dish_id,
                    date
                };
                string payload = JsonSerializer.Serialize(body);

                request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode.ToString() == "Unauthorized")
                    {
                        var freshRequest = new HttpRequestMessage(HttpMethod.Post, uri);
                        freshRequest.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                        response = await RetryRequest(freshRequest);
                        if (!response.IsSuccessStatusCode)
                        {
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
            catch (Exception ex)
            {
                return "{success: false}";
            }
        }

        public async Task<string> GetStock()
        {
            try
            {
                string user_id = await _userService.getUserId();
                Uri uri = new Uri(host + "/stock/" + user_id);
                var request = new HttpRequestMessage(HttpMethod.Get, uri);

                string token = await _tokenHandler.GetAccessToken();
                request.Headers.Add("token", token);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode.ToString() == "Unauthorized")
                    {
                        var freshRequest = new HttpRequestMessage(HttpMethod.Get, uri);
                        response = await RetryRequest(freshRequest);
                        if (!response.IsSuccessStatusCode)
                        {
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
            catch (Exception ex)
            {
                return "{success: false}";
            }
        }

        public async Task<string> EditStock(int ingredient_id, float amount)
        {
            try
            {
                string user_id = await _userService.getUserId();
                Uri uri = new Uri(host + "/stock/edit");
                var request = new HttpRequestMessage(HttpMethod.Post, uri);

                string token = await _tokenHandler.GetAccessToken();
                request.Headers.Add("token", token);

                var body = new
                {
                    user_id,
                    ingredient_id,
                    amount
                };
                string payload = JsonSerializer.Serialize(body);

                request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode.ToString() == "Unauthorized")
                    {
                        var freshRequest = new HttpRequestMessage(HttpMethod.Post, uri);
                        freshRequest.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                        response = await RetryRequest(freshRequest);
                        if (!response.IsSuccessStatusCode)
                        {
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
            catch (Exception ex)
            {
                return "{success: false}";
            }
        }

        public async Task<string> EditStock(int ingredient_id, int unit_id, string unit)
        {
            try
            {
                string user_id = await _userService.getUserId();
                Uri uri = new Uri(host + "/stock/edit");
                var request = new HttpRequestMessage(HttpMethod.Post, uri);

                string token = await _tokenHandler.GetAccessToken();
                request.Headers.Add("token", token);
                var unitInfo = new
                {
                    unit_id,
                    unit
                };
                var body = new
                {
                    user_id,
                    ingredient_id,
                    unitInfo
                };
                string payload = JsonSerializer.Serialize(body);

                request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode.ToString() == "Unauthorized")
                    {
                        var freshRequest = new HttpRequestMessage(HttpMethod.Post, uri);
                        freshRequest.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                        response = await RetryRequest(freshRequest);
                        if (!response.IsSuccessStatusCode)
                        {
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
            catch (Exception ex)
            {
                return "{success: false}";
            }
        }
    }
}
