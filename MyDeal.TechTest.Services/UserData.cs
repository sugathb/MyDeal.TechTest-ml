using Newtonsoft.Json;

namespace MyDeal.TechTest.Services
{
    public class UserData
    {
        [JsonProperty("data")]
        public User Data { get; set; }
    }
}