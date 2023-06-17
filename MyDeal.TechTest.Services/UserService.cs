using MyDeal.TechTest.Core.Infrastructure;
using System.Threading.Tasks;
using MyDeal.TechTest.Core.Models;

namespace MyDeal.TechTest.Services
{
    public class UserService: IUserService
    {
        private readonly IUserDetailsClient _userDetailsClient;

        public UserService(IUserDetailsClient userDetailsClient)
        {
            _userDetailsClient = userDetailsClient;
        }

        public async Task<UserData> GetUserDetails(string userId)
        {
            return await _userDetailsClient.GetUserDetailsAsync(userId);
        }
    }
}
