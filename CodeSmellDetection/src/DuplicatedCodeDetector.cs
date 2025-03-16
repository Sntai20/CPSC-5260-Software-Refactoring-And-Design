namespace CodeSmellDetection;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CodeSmellDetection.Models;
using CodeSmellDetection.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// Detects duplicated code within a given file content using Jaccard similarity.
/// </summary>
internal class DuplicatedCodeDetector(
    IOptions<DuplicatedCodeDetectorOptions> options,
    ILogger<DuplicatedCodeDetector> logger)
{
    private readonly IOptions<DuplicatedCodeDetectorOptions> options = options;
    private readonly ILogger<DuplicatedCodeDetector> logger = logger;

    /// <summary>
    /// Detects duplicated code within the provided file contents.
    /// </summary>
    /// <param name="fileContents">The contents of the file to analyze for duplicated code.</param>
    /// <returns>A <see cref="CodeSmell"/> object if duplicated code is detected; otherwise, <c>null</c>.</returns>
    public CodeSmell Detect(string fileContents)
    {
        var functions = ExtractFunctions(fileContents);
        var functionContents = new List<string>();

        foreach (var function in functions)
        {
            var content = ExtractFunctionContent(function);
            functionContents.Add(content);
        }

        for (int i = 0; i < functionContents.Count; i++)
        {
            var currentContent = functionContents[i];
            for (int j = i + 1; j < functionContents.Count; j++)
            {
                var comparisonContent = functionContents[j];
                var jaccardSimilarity = CalculateJaccardSimilarity(currentContent, comparisonContent);

                if (jaccardSimilarity >= this.options.Value.JaccardThreshold)
                {
                    this.logger.LogWarning($"Duplicated code detected between functions {i + 1} and {j + 1} with Jaccard similarity {jaccardSimilarity}.");
                    return new CodeSmell
                    {
                        Type = CodeSmellType.DuplicatedCode,
                        Description = "Duplicated code detected within the file.",
                        LineNumber = i + 1,
                        StartLine = i + 1,
                        EndLine = j + 1,
                        Code = fileContents,
                    };
                }
            }
        }

        this.logger.LogInformation("Duplicated code not detected.");
        return null;
    }

    internal static List<string> ExtractFunctions(string fileContents)
    {
        var functions = new List<string>();
        var functionPattern = @"(public|private|protected|internal|static|async)?\s*(void|int|string|bool|Task|List<.*?>|Dictionary<.*?>|IEnumerable<.*?>|IList<.*?>|IDictionary<.*?>|ICollection<.*?>|IReadOnlyList<.*?>|IReadOnlyDictionary<.*?>|IReadOnlyCollection<.*?>|IQueryable<.*?>|IAsyncEnumerable<.*?>|IAsyncEnumerator<.*?>|IAsyncDisposable<.*?>)?\s+\w+\s*\(.*?\)\s*{";
        var matches = Regex.Matches(fileContents, functionPattern, RegexOptions.Singleline);

        foreach (Match match in matches)
        {
            functions.Add(match.Value.Trim());
        }

        return functions;
    }

    internal static string ExtractFunctionContent(string function)
    {
        var startIndex = function.IndexOf('{') + 1;
        var endIndex = function.LastIndexOf('}');
        var content = function.Substring(startIndex, endIndex - startIndex).Trim();

        // Normalize indentation
        var lines = content.Split([Environment.NewLine], StringSplitOptions.None);
        var trimmedLines = lines.Select(line => line.TrimStart());
        return string.Join(Environment.NewLine, trimmedLines);
    }

    internal static double CalculateJaccardSimilarity(string content1, string content2)
    {
        var separators = new char[] { ' ', '\t', '(', ')', '{', '}', ';', ',', '.', '\n', '\r', '\f', '\v', '[', ']', '<', '>', ':', '"', '\'' };
        var words1 = new HashSet<string>(content1.Split(
            separators,
            StringSplitOptions.RemoveEmptyEntries));
        var words2 = new HashSet<string>(content2.Split(
            separators,
            StringSplitOptions.RemoveEmptyEntries));

        // Remove common programming keywords and symbols that should not affect similarity.
        var commonKeywords = new HashSet<string> { "int", "string", "=" };
        words1.ExceptWith(commonKeywords);
        words2.ExceptWith(commonKeywords);

        var intersectionCount = words1.Intersect(words2).Count();
        var unionCount = words1.Union(words2).Count();

        return unionCount == 0 ? 0.0 : (double)intersectionCount / unionCount;
    }
}