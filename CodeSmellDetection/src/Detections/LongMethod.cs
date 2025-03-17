namespace CodeSmellDetection.Detections;

using System.Text.RegularExpressions;
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
            var trimmedLine = RemoveComments(line.Trim());

            if (IsMethodDeclaration(trimmedLine))
            {
                if (inMethod && methodLineCount >= methodLineCountThreshold)
                {
                    this.logger.LogInformation($"Long Method Detected from line {methodStartLine} to {currentLineNumber}.");
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
                    this.logger.LogWarning($"Long Method Detected from line {methodStartLine} to {currentLineNumber}.");
                    codeSmells.Add(CreateCodeSmell(fileContents, methodStartLine, currentLineNumber));
                }

                inMethod = false;
            }
        }

        if (codeSmells.Count == 0)
        {
            this.logger.LogInformation("No Long Methods Detected.");
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
            Description = $"Long Method Detected from line {startLine} to {endLine}.",
            LineNumber = startLine,
            StartLine = startLine,
            EndLine = endLine,
            Code = fileContents,
        };
    }

    private static string RemoveComments(string line)
    {
        return Regex.Replace(line, @"//.*?$|/\*.*?\*/", string.Empty, RegexOptions.Singleline);
    }
}