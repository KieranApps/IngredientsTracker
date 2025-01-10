using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace IngredientsTracker.Helpers
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly TokenHandler _tokenHandler;
        //string host = 

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _tokenHandler = new TokenHandler();
        }

        public async Task<bool> CheckTokensAreValid(string refreshToken) // Purely valid on the server/in date. Already been read in the app storage
        {
            // Just check refresh throught the refresh endpoint
            // If refresh valid, just renew access regardless. If not, return false and stay in log in screen
            var request = new HttpRequestMessage(HttpMethod.Get, _httpClient.BaseAddress + "/refresh");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshToken);
            var response = await _httpClient.SendAsync(request);
            return true;
        }
    }
}
