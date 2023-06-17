using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace MyDeal.TechTest.Services
{
    public class UserData
    {
        [JsonPropertyName("data")]
        public User Data { get; set; }
    }
}