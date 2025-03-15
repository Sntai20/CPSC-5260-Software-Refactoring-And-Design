using CodeSmellDetection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceDefaults;

var hostApplicationBuilder = Host.CreateApplicationBuilder(args)
    .AddServiceDefaults();

hostApplicationBuilder.Services.AddCodeSmellDetectionService();

IHost? host = hostApplicationBuilder.Build();

try
{
    _ = host.Services.GetRequiredService<ICodeSmellDetectionService>().DetectAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}

await host.RunAsync();