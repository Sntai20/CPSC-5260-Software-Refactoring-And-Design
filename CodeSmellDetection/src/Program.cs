using CodeSmellDetection;
using CodeSmellDetection.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        _ = config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        _ = services.AddOptions<LongMethodDetectorOptions>()
                    .Bind(context.Configuration.GetSection("LongMethodDetectorOptions"));
        _ = services.AddOptions<LongParameterListDetectorOptions>()
                    .Bind(context.Configuration.GetSection("LongParameterListDetectorOptions"));
        _ = services.AddSingleton<LongMethodDetector>();
        _ = services.AddSingleton<LongParameterListDetector>();
        _ = services.AddSingleton<DuplicatedCodeDetector>();
    })
    .Build();

var configuration = host.Services.GetRequiredService<IConfiguration>();
var longMethodDetector = host.Services.GetRequiredService<LongMethodDetector>();
var longParameterListDetector = host.Services.GetRequiredService<LongParameterListDetector>();
var duplicatedCodeDetector = host.Services.GetRequiredService<DuplicatedCodeDetector>();

try
{
    string? pathToCodeFile = configuration["PathToCodeFile"] ?? throw new InvalidOperationException("Path to code file not found in configuration.");
    string fileContents = File.ReadAllText(pathToCodeFile);

    longMethodDetector.DetectLongMethods(fileContents);
    longParameterListDetector.DetectLongParameterLists(fileContents);
    duplicatedCodeDetector.DetectDuplicatedCode(fileContents);
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}

host.Run();