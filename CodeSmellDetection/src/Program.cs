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
    .AddSingleton<DuplicatedCodeDetector>()
    .AddSingleton<CodeSmellDetectionService>();

IHost? host = hostApplicationBuilder.Build();

try
{
    _ = host.Services.GetRequiredService<CodeSmellDetectionService>().Detect();
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}

host.Run();