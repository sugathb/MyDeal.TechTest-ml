using System.Threading.Tasks;
using MyDeal.TechTest.Core.Models;

namespace MyDeal.TechTest.Services;

public interface IUserService
{
    Task<UserData> GetUserDetails(string userId);
}