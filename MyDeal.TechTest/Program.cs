
using Microsoft.Net.Http.Headers;
using MyDeal.TechTest.Services;

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
app.MapForwarder("/{**catch-all}", app.Configuration["ProxyTo"]).Add(static builder => ((RouteEndpointBuilder)builder).Order = int.MaxValue);

app.Run();


// Add services to the container.
void ConfigureServices()
{
    builder.Services.AddSystemWebAdapters();
    builder.Services.AddHttpForwarder();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddControllersWithViews();

    builder.Services.AddHttpClient<IUserDetailsClient, UserDetailsClient>(httpClient =>
    {
        httpClient.BaseAddress = new Uri("https://reqres.in/api/users/");
        httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
        httpClient.Timeout = TimeSpan.FromMinutes(1);
    });
}
