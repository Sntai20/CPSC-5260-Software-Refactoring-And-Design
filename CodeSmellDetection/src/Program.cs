using CodeSmellDetection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceDefaults;

var hostApplicationBuilder = Host.CreateApplicationBuilder(args)
    .AddServiceDefaults();

hostApplicationBuilder.Configuration.AddUserSecrets<Program>();
hostApplicationBuilder.Services.AddCodeSmellDetectionService();

IHost? host = hostApplicationBuilder.Build();

try
{
    _ = host.Services.GetRequiredService<ICodeSmellDetectionService>().DetectAsync();
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}

await host.RunAsync();