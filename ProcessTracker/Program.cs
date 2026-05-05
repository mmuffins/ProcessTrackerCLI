using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProcessTracker.Core.Helpers;
using ProcessTracker.Core.HttpRequests;
using ProcessTracker.Core.Interfaces;
using ProcessTracker.Core.Menus;
using RestSharp;


string GetConfigFilePath()
{
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    {
        return GetConfigFilePathLinux();
    }

    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
        return GetConfigFilePathWindows();
    }

    throw new PlatformNotSupportedException("Unsupported platform.");
}

string GetConfigFilePathLinux()
{
    // Priority 1: Environment variable
    var configPathEnv = Environment.GetEnvironmentVariable("PROCESSTRACKER_APPSETTINGS_PATH");
    if (!string.IsNullOrEmpty(configPathEnv) && File.Exists(configPathEnv))
    {
        return configPathEnv;
    }

    // Priority 2: XDG_CONFIG_HOME
    var xdgConfigHome = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
    if (!string.IsNullOrEmpty(xdgConfigHome))
    {
        var xdgConfigPath = Path.Combine(xdgConfigHome, "processtracker", "appsettings.json");
        if (File.Exists(xdgConfigPath))
        {
            return xdgConfigPath;
        }
    }

    // Priority 3: Check if running as root
    if (Environment.UserName == "root")
    {
        var etcConfigPath = "/etc/processtracker/appsettings.json";
        if (File.Exists(etcConfigPath))
        {
            return etcConfigPath;
        }
    }

    // Priority 4: Default to user's home directory .config path
    var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    var userConfigPath = Path.Combine(homeDir, ".config", "processtracker", "appsettings.json");
    return userConfigPath;
}

string GetConfigFilePathWindows()
{
    // Priority 1: Environment variable
    var configPathEnv = Environment.GetEnvironmentVariable("PROCESSTRACKER_APPSETTINGS_PATH");
    if (!string.IsNullOrEmpty(configPathEnv) && File.Exists(configPathEnv))
    {
        return configPathEnv;
    }

    // Priority 2: Default path in ProgramData on system drive
    var systemDrive = Environment.GetEnvironmentVariable("SystemDrive") ?? "C:";
    var programDataPath = Path.Combine(systemDrive, "ProgramData", "ProcessTracker", "appsettings.json");

    return programDataPath;
}


// Configuration
var configFilePath = GetConfigFilePath();

var builder = new ConfigurationBuilder()
                .AddJsonFile(configFilePath, optional: false, reloadOnChange: true);
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
