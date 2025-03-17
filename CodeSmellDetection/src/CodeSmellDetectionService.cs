namespace CodeSmellDetection;

using CodeSmellDetection.Detections;
using CodeSmellDetection.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

/// <inheritdoc />
internal class CodeSmellDetectionService(
    StructuralDuplicateCode structuralDuplicateCodeDetector,
    LongMethod longMethodDetector,
    LongParameterList longParameterListDetector,
    ILogger<CodeSmellDetectionService> logger,
    IConfiguration configuration)
    : ICodeSmellDetectionService
{
    private readonly StructuralDuplicateCode structuralDuplicateCodeDetector = structuralDuplicateCodeDetector;
    private readonly LongMethod longMethodDetector = longMethodDetector;
    private readonly LongParameterList longParameterListDetector = longParameterListDetector;
    private readonly ILogger<CodeSmellDetectionService> logger = logger;
    private readonly IConfiguration configuration = configuration;

    /// <inheritdoc />
    public async Task<List<CodeSmell>> DetectAsync()
    {
        string? pathToCodeFile = this.configuration["PathToCodeFile"]
                                 ?? throw new InvalidOperationException("Path to fileContents file not found in configuration.");
        string fileContents = await File.ReadAllTextAsync(pathToCodeFile);

        var codeSmells = new List<CodeSmell>();

        List<CodeSmell>? duplicatedCodeSmells = await Task.Run(() => this.structuralDuplicateCodeDetector.Detect(fileContents));
        codeSmells.AddRange(duplicatedCodeSmells);

        var longMethodSmells = await Task.Run(() => this.longMethodDetector.Detect(fileContents));
        codeSmells.AddRange(longMethodSmells);

        var longParameterListSmells = await Task.Run(() => this.longParameterListDetector.Detect(fileContents));
        codeSmells.AddRange(longParameterListSmells);

        this.logger.LogInformation($"Detected {codeSmells.Count} code smells in the provided code file {pathToCodeFile}.");

        string fileName = Path.GetFileName(pathToCodeFile);
        foreach (var smell in codeSmells)
        {
            smell.FileName = fileName;
        }

        return codeSmells;
    }
}