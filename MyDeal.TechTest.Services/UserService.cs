using System.Threading.Tasks;

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
