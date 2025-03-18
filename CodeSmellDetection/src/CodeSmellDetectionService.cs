namespace CodeSmellDetection;

using CodeSmellDetection.Detections;
using CodeSmellDetection.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

/// <inheritdoc />
internal class CodeSmellDetectionService(
    StructuralDuplicateCode structuralDuplicateCodeDetection,
    LongMethod longMethodDetection,
    LongParameterList longParameterListDetection,
    ILogger<CodeSmellDetectionService> logger,
    IConfiguration configuration)
    : ICodeSmellDetectionService
{
    private readonly StructuralDuplicateCode structuralDuplicateCodeDetection = structuralDuplicateCodeDetection;
    private readonly LongMethod longMethodDetection = longMethodDetection;
    private readonly LongParameterList longParameterListDetection = longParameterListDetection;
    private readonly ILogger<CodeSmellDetectionService> logger = logger;
    private readonly IConfiguration configuration = configuration;

    /// <inheritdoc />
    public async Task<List<CodeSmell>> DetectAsync()
    {
        string? pathToCodeFile = this.configuration["PathToCodeFile"]
                                 ?? throw new InvalidOperationException("Path to fileContents file not found in configuration.");
        string fileContents = await File.ReadAllTextAsync(pathToCodeFile);
        string fileName = Path.GetFileName(pathToCodeFile);

        return await this.DetectAsync(fileName, fileContents);
    }

    /// <inheritdoc />
    public async Task<List<CodeSmell>> DetectAsync(string fileName, string fileContents)
    {
        var codeSmells = new List<CodeSmell>();

        List<CodeSmell>? structuralDuplicateCodeSmells = await Task.Run(() => this.structuralDuplicateCodeDetection.Detect(fileContents));
        codeSmells.AddRange(structuralDuplicateCodeSmells);

        List<CodeSmell>? longMethodSmells = await Task.Run(() => this.longMethodDetection.Detect(fileContents));
        codeSmells.AddRange(longMethodSmells);

        List<CodeSmell>? longParameterListSmells = await Task.Run(() => this.longParameterListDetection.Detect(fileContents));
        codeSmells.AddRange(longParameterListSmells);

        this.logger.LogInformation($"Detected {codeSmells.Count} code smells in the provided code file {fileName}.");

        foreach (var smell in codeSmells)
        {
            smell.FileName = fileName;
        }

        return codeSmells;
    }

    /// <inheritdoc />
    public Task<CodeSmell> RefactorAsync(CodeSmell codeSmell)
    {
        // TODO: Implement the refactoring logic here to call a service to get the refactored code.
        throw new NotImplementedException();
    }
}