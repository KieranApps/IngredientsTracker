using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

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

        public async Task<bool> CheckTokensAreValid(string refreshToken) // Purely valid on the server/in date. Already been read in the app storage
        {
            // Just check refresh throught the refresh endpoint
            // If refresh valid, just renew access regardless. If not, return false and stay in log in screen
            Uri uri = new Uri(host + "/auth/refresh");
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshToken);
            var response = await _httpClient.SendAsync(request);
            Debug.WriteLine(response);
            // Check status code and response. Will depend if we need to log out to force new session or update saved etc...

            return true;
        }
    }
}
