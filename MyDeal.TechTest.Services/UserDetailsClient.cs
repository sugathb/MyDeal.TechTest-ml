using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MyDeal.TechTest.Services
{
    public class UserDetailsClient : IUserDetailsClient
    {
        private readonly HttpClient _httpClient;

        public UserDetailsClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserData> GetUserDetailsAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<UserData>(id);
        }
    }
}
