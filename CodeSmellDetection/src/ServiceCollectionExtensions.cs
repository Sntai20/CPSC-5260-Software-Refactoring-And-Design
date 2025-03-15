namespace CodeSmellDetection;

using CodeSmellDetection.Options;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCodeSmellDetectionService(this IServiceCollection services)
    {
        _ = services.AddOptions<DuplicatedCodeDetectorOptions>()
            .BindConfiguration("DuplicatedCodeDetectorOptions");

        _ = services.AddOptions<LongMethodDetectorOptions>()
            .BindConfiguration("LongMethodDetectorOptions");

        _ = services.AddOptions<LongParameterListDetectorOptions>()
            .BindConfiguration("LongParameterListDetectorOptions");

        _ = services.AddSingleton<DuplicatedCodeDetector>()
            .AddSingleton<LongMethodDetector>()
            .AddSingleton<LongParameterListDetector>()
            .AddSingleton<CodeSmellDetectionService>();

        return services;
    }
}