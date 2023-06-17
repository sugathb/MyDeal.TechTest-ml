using System.Threading.Tasks;
using MyDeal.TechTest.Core.Models;

namespace MyDeal.TechTest.Core.Services;

public interface IUserService
{
    Task<UserData> GetUserDetails(string userId);
}