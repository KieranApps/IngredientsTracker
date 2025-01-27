using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace IngredientsTracker.Helpers
{
    public class TokenHandler
    {
        public async Task<string> GetAccessToken()
        {
            return await SecureStorage.GetAsync("AccessToken");
        }

        public async Task<string> GetRefreshToken()
        {
            return await SecureStorage.GetAsync("RefreshToken");
        }

        public async Task SaveAccessToken(string token)
        {
            await SecureStorage.SetAsync("AccessToken", token);
        }

        public async Task SaveRefreshToken(string token)
        {
            await SecureStorage.SetAsync("RefreshToken", token);
        }

        public void DeleteAccessToken()
        {
            SecureStorage.Remove("AccessToken");
        }
        public void DeleteRefreshToken()
        {
            SecureStorage.Remove("RefreshToken");
        }
    }
}
