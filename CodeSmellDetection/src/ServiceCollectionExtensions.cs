namespace CodeSmellDetection;

using CodeSmellDetection.Detections;
using CodeSmellDetection.Options;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCodeSmellDetectionService(this IServiceCollection services)
    {
        _ = services.AddOptions<StructuralDuplicateCodeOptions>()
            .BindConfiguration("StructuralDuplicateCodeOptions");

        _ = services.AddOptions<LongMethodDetectorOptions>()
            .BindConfiguration("LongMethodDetectorOptions");

        _ = services.AddOptions<LongParameterListDetectorOptions>()
            .BindConfiguration("LongParameterListDetectorOptions");

        _ = services.AddSingleton<StructuralDuplicateCode>()
            .AddSingleton<LongMethodDetector>()
            .AddSingleton<LongParameterListDetector>()
            .AddSingleton<ICodeSmellDetectionService, CodeSmellDetectionService>();

        return services;
    }
}