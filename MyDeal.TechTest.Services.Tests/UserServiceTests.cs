using Xunit;

namespace MyDeal.TechTest.Services.Tests
{
    public class UserServiceTests
    {
        [Fact]
        public void Test1()
        {
            UserService.WebRequestFactory = url => new TestWebRequest("{ \"data\": { \"first_name\": \"First\" } }");
            Assert.Equal("First", UserService.GetUserDetails("2").Data.FirstName);
        }
    }
}