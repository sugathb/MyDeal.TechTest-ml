using System.Text.Json.Serialization;

namespace MyDeal.TechTest.Core.Models
{
    public class UserData
    {
        [JsonPropertyName("data")]
        public User Data { get; set; }
    }
}