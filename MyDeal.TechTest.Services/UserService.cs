using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace MyDeal.TechTest.Services
{
    public static class UserService
    {
        public static Func<string, WebRequest> WebRequestFactory = WebRequest.Create;

        public static UserData GetUserDetails(string userId)
        {
            var response = WebRequestFactory("https://reqres.in/api/users/" + userId).GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string json = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<UserData>(json);
        }
    }
}
