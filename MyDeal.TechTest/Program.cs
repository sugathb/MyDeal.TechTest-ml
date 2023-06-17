using Microsoft.Net.Http.Headers;
using MyDeal.TechTest.Core.Infrastructure;
using MyDeal.TechTest.Core.Models;
using MyDeal.TechTest.Core.Queries;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);
ConfigureServices();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();
app.UseSystemWebAdapters();

app.MapDefaultControllerRoute();
app.MapForwarder("/{**catch-all}", app.Configuration["ProxyTo"])
    .Add(static builder => ((RouteEndpointBuilder)builder).Order = int.MaxValue);

app.Run();


// Add services to the container.
void ConfigureServices()
{
    builder.Services.Configure<SettingsOptions>(builder.Configuration.GetSection("Settings"));

    builder.Services.AddSystemWebAdapters();
    builder.Services.AddHttpForwarder();
    builder.Services.AddControllersWithViews();

    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(GetUserSettingsAsyncQuery)));

    builder.Services.AddHttpClient<IUserDetailsClient, UserDetailsClient>(httpClient =>
        {
            httpClient.BaseAddress = new Uri("https://reqres.in/api/users/");
            httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            httpClient.Timeout = TimeSpan.FromMinutes(1);
        })
        .SetHandlerLifetime(TimeSpan.FromMinutes(5))
        .AddPolicyHandler(GetRetryPolicy());
}

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    //Jittered Back-off provides a random factor to separate the retries in high concurrent requestscenarios
    var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 5);

    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(delay);
}
