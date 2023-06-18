# Notes
These notes contain a summary of improvements made to the application codebase. 

## Upgrade to .NET 6
`.NET Upgrade Assistant` is used to upgrade the solution to `.NET 6`. There were some manual refactoring required in order to make the application work after that.
https://dotnet.microsoft.com/en-us/platform/upgrade-assistant

## Use .NET inbuilt dependency injection framework
Used dependency injection to loosely couple services/components so that they are easier to maintain and unit test. 

## User Detail Typed Client to read user data
Added typed `HttpClient` to read user data from external web service using `IHttpClientFactory` with retrying policy. 

Register the client in `Program.cs`,
```csharp
var userDetailsClientSettings = builder.Configuration.GetSection("UserDetailsClientSettings").Get<UserDetailsClientSettings>();

if (userDetailsClientSettings != null)
{
    if (string.IsNullOrWhiteSpace(userDetailsClientSettings.BaseAddress))
    {
        throw new KeyNotFoundException("UserDetailsClientSettings requires a BaseAddress");
    }
    
    builder.Services.AddHttpClient<IUserDetailsClient, UserDetailsClient> (httpClient =>
    {
        httpClient.BaseAddress = new Uri(userDetailsClientSettings.BaseAddress);
        httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
        httpClient.Timeout = TimeSpan.FromMinutes(userDetailsClientSettings.ClientTimeoutInMinutes ?? defaultClientTimeoutInMinutes);
    }).SetHandlerLifetime(TimeSpan.FromMinutes(userDetailsClientSettings.MessageHandlerLifeTimeInMinutes ?? defaultMessageHandlerLifeTimeInMinutes))
    .AddPolicyHandler(GetRetryPolicy(userDetailsClientSettings.RetryDelayInSeconds ?? defaultRetryDelayInSeconds, userDetailsClientSettings.RetryCount ?? defaultRetryCount));
}

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryDelay, int retryCount)
{
    //Jittered Back-off provides a random factor to separate the retries in high concurrent request scenarios
    var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(retryDelay), retryCount: retryCount);
    return HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(delay);
}
```

User Details Client, 
```csharp
public class UserDetailsClient: IUserDetailsClient
{
    private readonly ILogger _logger = Log.ForContext<UserDetailsClient>();
    private readonly HttpClient _httpClient;
    
    public UserDetailsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<UserData> GetUserDetailsAsync(string id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<UserData>(id);
        }
        catch (HttpRequestException ex)
        {
            _logger.Error(ex, "Error occurred while getting user data for the user id :{UserId}.", id);
            throw;
        }
    }
}
```

## Introduced CQRS pattern with Mediatr
Introduced CQRS pattern to communicate between the controller and services layers that enhances the maintainability as it enforces the single responsibility SOLID principle.

Register the query handler in the `Program.cs`,
```CSharp
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(GetUserSettingsAsyncQuery)));
```
GetUserSettings query and handler,
```csharp
 public class GetUserSettingsAsyncQuery: IRequest<SettingsVm>
 {
     public string UserId { get; set; }
 }
 
 public class GetUserSettingsAsyncQueryHandler: IRequestHandler<GetUserSettingsAsyncQuery,SettingsVm>
 {
     private readonly IUserDetailsClient _userDetailsClient;
     private readonly SettingsOptions _settingsOptions;
     
     public GetUserSettingsAsyncQueryHandler(IUserDetailsClient userDetailsClient, IOptions <SettingsOptions> options)
     {
         _userDetailsClient = userDetailsClient;
         _settingsOptions = options.Value;
     }
     
     public async Task<SettingsVm> Handle(GetUserSettingsAsyncQuery request, CancellationToken cancellationToken)
     {
         var userData = await _userDetailsClient.GetUserDetailsAsync(request.UserId);
         
         return new SettingsVm
         {
             User = userData?.Data,
             Message = _settingsOptions.Message
         };
     }
 }
```

Call the query handler from the controller,
```csharp
public class SettingsController: Controller
{
    private readonly IMediator _mediator;
    private const string UserId = "2";
    
    public SettingsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    public async Task<ActionResult> Index()
    {
        var getUserDetailsQuery = new GetUserSettingsAsyncQuery
        {
            UserId = UserId
        };
        
        var response = await _mediator.Send(getUserDetailsQuery);
        return Ok(response);
    }
}
```

## Introduced Options Pattern
Introduced options pattern to provide strongly typed access to app settings,

In the `appsettings.json`,
```CSharp
 "Settings": {
    "Message": "My AJAX message!"
  },
```

Then define option type,
```CSharp
 public class SettingsOptions
 {
     public string Message { get; set; }
 }
```

In the `Program.cs`,
```CSharp
builder.Services.Configure<SettingsOptions>(builder.Configuration.GetSection("Settings"));
```

Then settings can be accessed using `IOptions`,
```csharp
 public class GetUserSettingsAsyncQueryHandler: IRequestHandler<GetUserSettingsAsyncQuery,SettingsVm>
 {
     private readonly IUserDetailsClient _userDetailsClient;
     private readonly SettingsOptions _settingsOptions;
     
     public GetUserSettingsAsyncQueryHandler(IUserDetailsClient userDetailsClient, IOptions<SettingsOptions> options)
     {
         _userDetailsClient = userDetailsClient;
         _settingsOptions = options.Value;
     }
     
     public async Task<SettingsVm> Handle(GetUserSettingsAsyncQuery request, CancellationToken cancellationToken)
     {
         var userData = await _userDetailsClient.GetUserDetailsAsync(request.UserId);
         return new SettingsVm
         {
             User = userData?.Data,
             Message = _settingsOptions.Message
         };
     }
 }
```

## Error Handling
Added error handling in the code where required, 
```csharp
public async Task<UserData> GetUserDetailsAsync(string id)
{
    try
    {
        return await _httpClient.GetFromJsonAsync<UserData>(id);
    }
    catch (HttpRequestException ex)
    {
        _logger.Error(ex, "Error occurred while getting user data for the user id :{UserId}.", id);
        throw;
    }
}
```

Also introduced a basic error handling middleware that can be expanded later,
```csharp
public class ErrorHandlingMiddleware: IMiddleware
{
    private readonly ILogger _logger = Log.ForContext<UserDetailsClient>();
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var problemDetail = new ProblemDetail
        {
            Type = "Server Error",
            Title = "Server Error",
            Status = 500,
            Detail = "An internal server error has occurred"
        };
        
        var json = JsonConvert.SerializeObject(problemDetail);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
        await context.Response.WriteAsync(json);
    }
}
```

In the `Program.cs`,
```CSharp
builder.Services.AddTransient<ErrorHandlingMiddleware>();
```

## Logging 
Introduced structured logging using `Serilog`,
In the `Program.cs`,
```CSharp
//TODO: Configure to sync logs to some other log management tool such as Datadog
builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console());
```

Usage,
```csharp
 private readonly ILogger _logger = Log.ForContext<UserDetailsClient> ();
 
 public async Task<UserData> GetUserDetailsAsync(string id)
 {
     try
     {
         return await _httpClient.GetFromJsonAsync<UserData>(id);
     }
     catch (HttpRequestException ex)
     {
         _logger.Error(ex, "Error occurred while getting user data for the user id :{UserId}.", id);
         throw;
     }
 }
```


## Unit Tests
Unit tests added for `GetUserSettingsAsyncQueryHandler` and `UserDetailsClient`. In order to unit test the `UserDetailsClient`, the underline `HttpMessageHandler` is required to be Mocked,
```csharp
private static HttpClient GetHttpClientMock()
{
    const string responseContent = "{\"data\":{\"id\":2,\"email\":\"janet.weaver@reqres.in\",\"first_name\":\"Janet\",\"last_name\":\"Weaver\"}}";
    var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
    
    httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>> ("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(new HttpResponseMessage
    {
        StatusCode = HttpStatusCode.OK,
        Content = new StringContent(responseContent)
    }).Verifiable();
    
    var httpClient = new HttpClient(httpMessageHandlerMock.Object);
    httpClient.BaseAddress = new Uri("http://localhost");
    return httpClient;
}
```


