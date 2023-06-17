using System.Threading.Tasks;

namespace MyDeal.TechTest.Services;

public interface IUserDetailsClient
{
    public Task<UserData> GetUserDetailsAsync(string id);
}