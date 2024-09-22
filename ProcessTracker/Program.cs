using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProcessTracker.Core.Helpers;
using ProcessTracker.Core.HttpRequests;
using ProcessTracker.Core.Interfaces;
using ProcessTracker.Core.Menus;
using RestSharp;


// Configuration
var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
IConfiguration configuration = builder.Build();

var appsettings = configuration.GetSection("AppSettings").Get<AppSettings>();

// Services
var services = new ServiceCollection()
    .AddLogging(loggingBuilder =>
    {
        loggingBuilder.AddConsole();
    })
    .AddSingleton(configuration)
    .AddSingleton<IRestClient, RestClient>(provide => new RestClient("http://localhost:" + appsettings.HttpPort))
    .AddSingleton<IHttpService, HttpService>()
    .AddSingleton<ProcessMenu>()
    .BuildServiceProvider();

var logger = (services.GetService<ILoggerFactory>() ?? throw new InvalidOperationException())
    .CreateLogger<Program>();

//logger.LogInformation($"Starting application at: {DateTime.Now}");

// create object of the process menu
ConsoleMenu menu = services.GetService<ProcessMenu>();

// display initial menu
await menu.Select();
