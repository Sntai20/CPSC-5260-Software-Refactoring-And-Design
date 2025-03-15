﻿namespace CodeSmellDetection;

using CodeSmellDetection.Models;
using CodeSmellDetection.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// Detects long methods in the provided file contents.
/// </summary>
internal class LongMethodDetector(
    IOptions<LongMethodDetectorOptions> options,
    ILogger<LongMethodDetector> logger)
{
    private readonly IOptions<LongMethodDetectorOptions> options = options;
    private readonly ILogger<LongMethodDetector> logger = logger;

    public CodeSmell Detect(string fileContents)
    {
        int methodLineCount = 0;
        bool inMethod = false;
        var methodLineCountThreshold = this.options.Value.MethodLineCountThreshold;

        foreach (var line in fileContents.Split(Environment.NewLine))
        {
            var trimmedLine = line.Trim();

            if (IsMethodDeclaration(trimmedLine))
            {
                if (inMethod && methodLineCount >= methodLineCountThreshold)
                {
                    this.logger.LogInformation("Long Method Detected.");
                    return CreateCodeSmell(fileContents, line.Length);
                }

                inMethod = true;
                methodLineCount = 0;
            }

            if (inMethod && !string.IsNullOrWhiteSpace(trimmedLine))
            {
                methodLineCount++;
            }

            if (trimmedLine == "}")
            {
                if (inMethod && methodLineCount >= methodLineCountThreshold)
                {
                    this.logger.LogWarning("Long Method Detected.");
                    return CreateCodeSmell(fileContents, line.Length);
                }

                inMethod = false;
            }
        }

        this.logger.LogInformation("Long Method Not Detected.");
        return null;
    }

    private static bool IsMethodDeclaration(string line)
    {
        return line.StartsWith("public", StringComparison.Ordinal) ||
               line.StartsWith("private", StringComparison.Ordinal) ||
               line.StartsWith("protected", StringComparison.Ordinal) ||
               line.StartsWith("internal", StringComparison.Ordinal);
    }

    private static CodeSmell CreateCodeSmell(string fileContents, int lineLength)
    {
        return new CodeSmell
        {
            Type = CodeSmellType.LongMethod,
            Description = "Long Method Detected",
            LineNumber = lineLength,
            Code = fileContents,
        };
    }
}