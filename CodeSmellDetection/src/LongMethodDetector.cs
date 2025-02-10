namespace CodeSmellDetection;

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

    /// <summary>
    /// Detects methods in the provided file contents that exceed a certain line count.
    /// </summary>
    /// <param name="fileContents">The contents of the file to analyze.</param>
    public void DetectLongMethods(string fileContents)
    {
        int methodLineCount = 0;
        bool inMethod = false;
        var methodLineCountThreshold = this.options.Value.MethodLineCountThreshold;

        foreach (var line in fileContents.Split(Environment.NewLine))
        {
            if (line.Trim().StartsWith("public", StringComparison.Ordinal) ||
                line.Trim().StartsWith("private", StringComparison.Ordinal) ||
                line.Trim().StartsWith("protected", StringComparison.Ordinal) ||
                line.Trim().StartsWith("internal", StringComparison.Ordinal))
            {
                if (inMethod && methodLineCount >= methodLineCountThreshold)
                {
                    this.logger.LogInformation("Long Method Detected");
                }

                inMethod = true;
                methodLineCount = 0;
            }

            if (inMethod && !string.IsNullOrWhiteSpace(line))
            {
                methodLineCount++;
            }

            if (line.Trim() == "}")
            {
                if (inMethod && methodLineCount >= methodLineCountThreshold)
                {
                    this.logger.LogWarning("Long Method Detected");
                }

                inMethod = false;
            }
        }
    }
}