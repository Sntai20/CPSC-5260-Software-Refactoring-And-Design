using CodeRefactorService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var hostApplicationBuilder = Host.CreateApplicationBuilder(args);

IHost? host = hostApplicationBuilder.Build();

try
{
    _ = host.Services.GetRequiredService<ICodeRefactorService>().RefactorAsync();
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}

await host.RunAsync();