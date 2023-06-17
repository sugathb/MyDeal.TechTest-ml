using System.IO;
using System.Net;
using System.Text;

namespace MyDeal.TechTest.Services.Tests
{
    public class TestWebResponse : WebResponse
    {
        private readonly string _responseData;

        public TestWebResponse(string responseData)
        {
            _responseData = responseData;
        }

        public override Stream GetResponseStream()
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(_responseData));
        }
    }
}