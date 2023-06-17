using MyDeal.TechTest.Core.Models;

namespace MyDeal.TechTest.Core.Infrastructure;

public interface IUserDetailsClient
{
    public Task<UserData> GetUserDetailsAsync(string id);
}