using Microsoft.Net.Http.Headers;
using MyDeal.TechTest.Core.Infrastructure;
using MyDeal.TechTest.Core.Middleware;
using MyDeal.TechTest.Core.Models;
using MyDeal.TechTest.Core.Queries;
using MyDeal.TechTest.Settings;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//TODO: Configure to sync logs to some other log management tool such as Datadog
builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console());

ConfigureServices();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapDefaultControllerRoute();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.Run();


// Add services to the container.
void ConfigureServices()
{
    builder.Services.Configure<SettingsOptions>(builder.Configuration.GetSection("Settings"));
    builder.Services.AddSystemWebAdapters();
    builder.Services.AddHttpForwarder();
    builder.Services.AddControllersWithViews();
    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(GetUserSettingsAsyncQuery)));
    builder.Services.AddTransient<ErrorHandlingMiddleware>();
    ConfigureGetUserDetailsClient();
}

void ConfigureGetUserDetailsClient()
{
    const int defaultClientTimeoutInMinutes = 1;
    const int defaultMessageHandlerLifeTimeInMinutes = 5;
    const int defaultRetryDelayInSeconds = 1;
    const int defaultRetryCount = 3;

    var userDetailsClientSettings = builder.Configuration.GetSection("UserDetailsClientSettings").Get<UserDetailsClientSettings>();

    if (userDetailsClientSettings != null)
    {
        if (string.IsNullOrWhiteSpace(userDetailsClientSettings.BaseAddress))
        {
            throw new KeyNotFoundException("UserDetailsClientSettings requires a BaseAddress");
        }

        builder.Services.AddHttpClient<IUserDetailsClient, UserDetailsClient>(httpClient =>
            {
                httpClient.BaseAddress = new Uri(userDetailsClientSettings.BaseAddress);
                httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
                httpClient.Timeout = TimeSpan.FromMinutes(userDetailsClientSettings.ClientTimeoutInMinutes ?? defaultClientTimeoutInMinutes);
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(userDetailsClientSettings.MessageHandlerLifeTimeInMinutes ?? defaultMessageHandlerLifeTimeInMinutes))
            .AddPolicyHandler(GetRetryPolicy(userDetailsClientSettings.RetryDelayInSeconds ?? defaultRetryDelayInSeconds,
                userDetailsClientSettings.RetryCount ?? defaultRetryCount));
    }
}

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryDelay, int retryCount)
{
    //Jittered Back-off provides a random factor to separate the retries in high concurrent request scenarios
    var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(retryDelay), retryCount: retryCount);

    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(delay);
}
