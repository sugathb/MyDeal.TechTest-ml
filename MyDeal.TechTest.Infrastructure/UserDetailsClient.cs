using System.Net.Http.Json;
using MyDeal.TechTest.Core;
using MyDeal.TechTest.Core.Models;

namespace MyDeal.TechTest.Infrastructure
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
