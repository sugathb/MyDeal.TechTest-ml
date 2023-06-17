using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MyDeal.TechTest.Core.Models;
using Serilog;

namespace MyDeal.TechTest.Core.Infrastructure
{
    public class UserDetailsClient : IUserDetailsClient
    {
        private readonly ILogger _logger = Log.ForContext<UserDetailsClient>();
        private readonly HttpClient _httpClient;

        public UserDetailsClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserData> GetUserDetailsAsync(string id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<UserData>(id);
            }
            catch (HttpRequestException ex)
            {
                _logger.Error(ex, "Error occurred while getting user data for the user id :{UserId}.", id);
                throw;
            }
        }
    }
}
