
namespace IngredientsTracker.Helpers
{
    public class UserService
    {

        public async Task<string> GetUserName()
        {
            return await SecureStorage.GetAsync("Name");
        }

        public async Task SaveUserName(string name)
        {
            await SecureStorage.SetAsync("Name", name);
        }

        public void DeleteUserName()
        {
            SecureStorage.Remove("Name");
        }

        public async Task<string> GetUserEmail()
        {
            return await SecureStorage.GetAsync("Email");
        }

        public async Task SaveUserEmail(string email)
        {
            await SecureStorage.SetAsync("Email", email);
        }

        public void DeleteUserEmail()
        {
            SecureStorage.Remove("Email");
        }

        public async Task<string> getUserId()
        {
            return await SecureStorage.GetAsync("Id");
        }

        public async Task SaveUserId(string id)
        {
            await SecureStorage.SetAsync("Id", id);
        }

        public void DeleteUserId()
        {
            SecureStorage.Remove("Id");
        }
    }
}
