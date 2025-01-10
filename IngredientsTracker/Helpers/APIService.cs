using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

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

        public async Task<bool> CheckTokensAreValid(string refreshToken, string accessToken) // Purely valid on the server/in date. Already been read in the app storage
        {
            return true;
        }
    }
}
