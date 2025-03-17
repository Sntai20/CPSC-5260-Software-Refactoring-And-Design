namespace CodeSmellDetection;

using CodeSmellDetection.Models;
using CodeSmellDetection.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// Detects long methods in the provided file contents.
/// </summary>
internal class LongMethod(
    IOptions<LongMethodOptions> options,
    ILogger<LongMethod> logger)
{
    private readonly IOptions<LongMethodOptions> options = options;
    private readonly ILogger<LongMethod> logger = logger;

    // TODO: Ignore comments from the fileContents.
    public List<CodeSmell> Detect(string fileContents)
    {
        var codeSmells = new List<CodeSmell>();
        int methodLineCount = 0;
        int currentLineNumber = 0;
        int methodStartLine = 0;
        bool inMethod = false;
        var methodLineCountThreshold = this.options.Value.MethodLineCountThreshold;

        foreach (var line in fileContents.Split(Environment.NewLine))
        {
            currentLineNumber++;
            var trimmedLine = line.Trim();

            if (IsMethodDeclaration(trimmedLine))
            {
                if (inMethod && methodLineCount >= methodLineCountThreshold)
                {
                    // TODO: Improve log statement to be data dense.
                    this.logger.LogInformation("Long Method Detected.");
                    codeSmells.Add(CreateCodeSmell(fileContents, methodStartLine, currentLineNumber));
                }

                inMethod = true;
                methodLineCount = 0;
                methodStartLine = currentLineNumber;
            }

            if (inMethod && !string.IsNullOrWhiteSpace(trimmedLine))
            {
                methodLineCount++;
            }

            if (trimmedLine == "}")
            {
                if (inMethod && methodLineCount >= methodLineCountThreshold)
                {
                    // TODO: Improve log statement to be data dense.
                    this.logger.LogWarning("Long Method Detected.");
                    codeSmells.Add(CreateCodeSmell(fileContents, methodStartLine, currentLineNumber));
                }

                inMethod = false;
            }
        }

        if (codeSmells.Count == 0)
        {
            this.logger.LogInformation("Long Method Not Detected.");
        }

        return codeSmells;
    }

    private static bool IsMethodDeclaration(string line)
    {
        return line.StartsWith("public", StringComparison.Ordinal) ||
               line.StartsWith("private", StringComparison.Ordinal) ||
               line.StartsWith("protected", StringComparison.Ordinal) ||
               line.StartsWith("internal", StringComparison.Ordinal);
    }

    private static CodeSmell CreateCodeSmell(string fileContents, int startLine, int endLine)
    {
        return new CodeSmell
        {
            // TODO: Improve the description.
            Type = CodeSmellType.LongMethod,
            Description = "Long Method Detected",
            LineNumber = startLine,
            StartLine = startLine,
            EndLine = endLine,
            Code = fileContents,
        };
    }
}