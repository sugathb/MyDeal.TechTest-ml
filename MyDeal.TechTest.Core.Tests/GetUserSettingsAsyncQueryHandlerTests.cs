using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using MyDeal.TechTest.Core.Infrastructure;
using MyDeal.TechTest.Core.Models;
using MyDeal.TechTest.Core.Queries;
using Shouldly;
using Xunit;

namespace MyDeal.TechTest.Core.Tests
{
    public class GetUserSettingsAsyncQueryHandlerTests
    {
        private readonly Mock<IUserDetailsClient> _userDetailsClientMock;
        private readonly Mock<IOptions<SettingsOptions>> _settingsOptionsMock;

        public GetUserSettingsAsyncQueryHandlerTests()
        {
            _userDetailsClientMock = new Mock<IUserDetailsClient>();
            _settingsOptionsMock = new Mock<IOptions<SettingsOptions>>();
        }


        [Fact]
        public async Task ShouldReturnUserForValidRequest()
        {
            const long userId = 2;
            const string firstName = "Janet";
            const string lastName = "Weaver";
            const string email = "janet.weaver@reqres.in";
            const string message = "My Test Message";

            _userDetailsClientMock.Setup(x => x.GetUserDetailsAsync(It.IsAny<string>()))
                .ReturnsAsync(new UserData{Data = new User{Id = userId, FirstName = firstName, LastName = lastName, Email = email}});
            _settingsOptionsMock.Setup(x => x.Value).Returns(new SettingsOptions{Message = message});

            var getUserSettingsAsyncQuery = new GetUserSettingsAsyncQuery{UserId = userId.ToString()};
            var getUserSettingsAsyncQueryHandler = new GetUserSettingsAsyncQueryHandler(_userDetailsClientMock.Object, _settingsOptionsMock.Object);

            var response = await getUserSettingsAsyncQueryHandler.Handle(getUserSettingsAsyncQuery, CancellationToken.None);

            response.User.ShouldNotBe(null);
            response.Message.ShouldNotBe(null);
            response.User.Id.ShouldBe(userId);
            response.User.FirstName.ShouldBe(firstName);
            response.User.LastName.ShouldBe(lastName);
            response.User.Email.ShouldBe(email);
            response.Message.ShouldBe(message);
        }
    }
}
