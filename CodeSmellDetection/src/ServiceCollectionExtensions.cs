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

        _ = services.AddOptions<LongMethodOptions>()
            .BindConfiguration("LongMethodOptions");

        _ = services.AddOptions<LongParameterListOptions>()
            .BindConfiguration("LongParameterListOptions");

        _ = services.AddSingleton<StructuralDuplicateCode>()
            .AddSingleton<LongMethod>()
            .AddSingleton<LongParameterList>()
            .AddSingleton<ICodeSmellDetectionService, CodeSmellDetectionService>();

        return services;
    }
}