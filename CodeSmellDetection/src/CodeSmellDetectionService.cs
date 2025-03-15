namespace CodeSmellDetection;

using CodeSmellDetection.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

internal class CodeSmellDetectionService(
    DuplicatedCodeDetector duplicatedCodeDetector,
    LongMethodDetector longMethodDetector,
    LongParameterListDetector longParameterListDetector,
    ILogger<CodeSmellDetectionService> logger,
    IConfiguration configuration)
{
    private readonly DuplicatedCodeDetector duplicatedCodeDetector = duplicatedCodeDetector;
    private readonly LongMethodDetector longMethodDetector = longMethodDetector;
    private readonly LongParameterListDetector longParameterListDetector = longParameterListDetector;
    private readonly ILogger<CodeSmellDetectionService> logger = logger;
    private readonly IConfiguration configuration = configuration;

    public List<CodeSmell> Detect()
    {
        string? pathToCodeFile = this.configuration["PathToCodeFile"] ?? throw new InvalidOperationException("Path to fileContents file not found in configuration.");
        string fileContents = File.ReadAllText(pathToCodeFile);

        var codeSmells = new List<CodeSmell>();

        var duplicatedCodeSmells = this.duplicatedCodeDetector.Detect(fileContents);
        codeSmells.AddRange(duplicatedCodeSmells);

        var longMethodSmells = this.longMethodDetector.Detect(fileContents);
        codeSmells.AddRange(longMethodSmells);

        var longParameterListSmells = this.longParameterListDetector.Detect(fileContents);
        codeSmells.AddRange(longParameterListSmells);

        this.logger.LogInformation($"Detected {codeSmells.Count} fileContents smells in the provided fileContents.");

        return codeSmells;
    }
}