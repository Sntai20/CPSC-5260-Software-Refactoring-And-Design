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
        int braceCount = 0;
        var methodLineCountThreshold = this.options.Value.MethodLineCountThreshold;

        foreach (var line in fileContents.Split(Environment.NewLine))
        {
            var trimmedLine = RemoveComments(line.Trim());
            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                continue;
            }

            currentLineNumber++;

            if (IsClassDeclaration(trimmedLine))
            {
                continue;
            }

            if (IsMethodDeclaration(trimmedLine))
            {
                if (inMethod && methodLineCount >= methodLineCountThreshold)
                {
                    this.logger.LogWarning($"Long Method Detected from line {methodStartLine} to {currentLineNumber}.");
                    codeSmells.Add(CreateCodeSmell(fileContents, methodStartLine, currentLineNumber));
                }

                inMethod = true;
                methodLineCount = 0;
                methodStartLine = currentLineNumber;
                braceCount = 0;
                continue;
            }

            if (inMethod)
            {
                methodLineCount++;
                braceCount += trimmedLine.Count(c => c == '{');
                braceCount -= trimmedLine.Count(c => c == '}');

                if (braceCount == 0)
                {
                    if (methodLineCount >= methodLineCountThreshold)
                    {
                        this.logger.LogWarning($"Long Method Detected from line {methodStartLine} to {currentLineNumber}.");
                        codeSmells.Add(CreateCodeSmell(fileContents, methodStartLine, currentLineNumber));
                    }

                    inMethod = false;
                }
            }
        }

        if (codeSmells.Count == 0)
        {
            this.logger.LogInformation("Long Methods Not Detected.");
        }

        return codeSmells;
    }

    internal static bool IsClassDeclaration(string line)
    {
        return line.StartsWith("public class", StringComparison.Ordinal) ||
               line.StartsWith("private class", StringComparison.Ordinal) ||
               line.StartsWith("protected class", StringComparison.Ordinal) ||
               line.StartsWith("internal class", StringComparison.Ordinal);
    }

    internal static bool IsMethodDeclaration(string line)
    {
        return (line.StartsWith("public", StringComparison.Ordinal) ||
                line.StartsWith("private", StringComparison.Ordinal) ||
                line.StartsWith("protected", StringComparison.Ordinal) ||
                line.StartsWith("internal", StringComparison.Ordinal)) &&
                line.Contains("(") && line.Contains(")") &&
                !line.Contains("class");
    }

    internal static CodeSmell CreateCodeSmell(string fileContents, int startLine, int endLine)
    {
        return new CodeSmell
        {
            Type = CodeSmellType.LongMethod,
            Description = $"Long Method Detected from line {startLine} to {endLine}.",
            LineNumber = startLine,
            StartLine = startLine,
            EndLine = endLine,
            Code = fileContents,
        };
    }

    internal static string RemoveComments(string line)
    {
        return Regex.Replace(line, @"//.*?$|/\*.*?\*/", string.Empty, RegexOptions.Singleline);
    }
}