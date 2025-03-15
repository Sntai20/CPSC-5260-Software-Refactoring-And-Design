using CodeSmellDetection;
using CodeSmellDetection.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceDefaults;

var hostApplicationBuilder = Host.CreateApplicationBuilder(args)
    .AddServiceDefaults();

hostApplicationBuilder.Services.AddOptions<LongMethodDetectorOptions>()
    .BindConfiguration("LongMethodDetectorOptions");

hostApplicationBuilder.Services.AddOptions<LongParameterListDetectorOptions>()
    .BindConfiguration("LongParameterListDetectorOptions");

hostApplicationBuilder.Services.AddOptions<DuplicatedCodeDetectorOptions>()
    .BindConfiguration("DuplicatedCodeDetectorOptions");

hostApplicationBuilder.Services
    .AddSingleton<LongMethodDetector>()
    .AddSingleton<LongParameterListDetector>()
    .AddSingleton<DuplicatedCodeDetector>();

IHost? host = hostApplicationBuilder.Build();

try
{
    var configuration = host.Services.GetRequiredService<IConfiguration>();
    string? pathToCodeFile = configuration["PathToCodeFile"] ?? throw new InvalidOperationException("Path to code file not found in configuration.");
    string fileContents = File.ReadAllText(pathToCodeFile);

    var longMethodDetector = host.Services.GetRequiredService<LongMethodDetector>();
    var longParameterListDetector = host.Services.GetRequiredService<LongParameterListDetector>();
    var duplicatedCodeDetector = host.Services.GetRequiredService<DuplicatedCodeDetector>();

    longMethodDetector.Detect(fileContents);
    longParameterListDetector.Detect(fileContents);
    duplicatedCodeDetector.Detect(fileContents);
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}

host.Run();