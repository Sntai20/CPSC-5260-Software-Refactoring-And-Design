namespace CodeSmellDetection;

using Microsoft.Extensions.Logging;

internal class CodeSmellDetectionService(
    DuplicatedCodeDetector duplicatedCodeDetector,
    LongMethodDetector longMethodDetector,
    LongParameterListDetector longParameterListDetector,
    ILogger<CodeSmellDetectionService> logger)
{
    private readonly DuplicatedCodeDetector duplicatedCodeDetector = duplicatedCodeDetector;
    private readonly LongMethodDetector longMethodDetector = longMethodDetector;
    private readonly LongParameterListDetector longParameterListDetector = longParameterListDetector;
    private readonly ILogger<CodeSmellDetectionService> logger = logger;

    public List<CodeSmell> DetectCodeSmells(string code)
    {
        var codeSmells = new List<CodeSmell>();

        var duplicatedCodeSmells = this.duplicatedCodeDetector.Detect(code);
        codeSmells.AddRange(duplicatedCodeSmells);

        /*var longMethodSmells = this.longMethodDetector.Detect(code);
        codeSmells.AddRange(longMethodSmells);

        var longParameterListSmells = this.longParameterListDetector.Detect(code);
        codeSmells.AddRange(longParameterListSmells);*/

        this.logger.LogInformation($"Detected {codeSmells.Count} code smells in the provided code.");

        return codeSmells;
    }
}