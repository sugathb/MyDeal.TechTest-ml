namespace MyDeal.TechTest.Settings
{
    public class UserDetailsClientSettings
    {
        public string BaseAddress { get; set; }
        public int? ClientTimeoutInMinutes { get; set; }
        public int? MessageHandlerLifeTimeInMinutes { get; set; }
        public int? RetryDelayInSeconds { get; set; }
        public int? RetryCount { get; set; }
    }
}
