using System.Threading.Tasks;

namespace MyDeal.TechTest.Services;

public interface IUserService
{
    Task<UserData> GetUserDetails(string userId);
}