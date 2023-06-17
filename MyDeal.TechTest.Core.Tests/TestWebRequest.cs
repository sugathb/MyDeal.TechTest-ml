using System.Net;

namespace MyDeal.TechTest.Services.Tests
{
    public class TestWebRequest : WebRequest
    {
        private readonly string _responseData;

        public TestWebRequest(string responseData)
        {
            _responseData = responseData;
        }

        public override WebResponse GetResponse()
        {
            return new TestWebResponse(_responseData);
        }
    }
}