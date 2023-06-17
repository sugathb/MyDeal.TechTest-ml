using System;
using System.Text.Json.Serialization;

namespace MyDeal.TechTest.Core.Models
{
    public class User
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("avatar")]
        public Uri Avatar { get; set; }
    }
}