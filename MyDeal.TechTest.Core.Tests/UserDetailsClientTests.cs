using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using MyDeal.TechTest.Core.Infrastructure;
using Shouldly;
using Xunit;

namespace MyDeal.TechTest.Core.Tests
{
    public class UserDetailsClientTests
    {
        [Fact]
        public async Task ShouldReturnUserForValidRequest()
        {
            const long userId = 2;
            const string firstName = "Janet";
            const string lastName = "Weaver";
            const string email = "janet.weaver@reqres.in";
            var userDetailClient = new UserDetailsClient(GetHttpClientMock());

            var response = await userDetailClient.GetUserDetailsAsync("2");

            response.Data.ShouldNotBe(null);
            response.Data.Id.ShouldBe(userId);
            response.Data.FirstName.ShouldBe(firstName);
            response.Data.LastName.ShouldBe(lastName);
            response.Data.Email.ShouldBe(email);
        }

        private static HttpClient GetHttpClientMock()
        {
            const string responseContent = "{\"data\":{\"id\":2,\"email\":\"janet.weaver@reqres.in\",\"first_name\":\"Janet\",\"last_name\":\"Weaver\"}}";

            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent)
                })
                .Verifiable();

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);
            httpClient.BaseAddress = new Uri("http://localhost");
            return httpClient;
        }

    }
}
