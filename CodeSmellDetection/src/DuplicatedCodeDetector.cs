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
        var functionNames = new HashSet<string>();

        foreach (var function in functions)
        {
            var content = ExtractFunctionContent(function);
            functionContents.Add(content);

            var functionName = ExtractFunctionName(function);
            if (!functionNames.Add(functionName))
            {
                this.logger.LogWarning($"Duplicate function name detected: {functionName}.");
                return new CodeSmell
                {
                    Type = CodeSmellType.DuplicatedCode,
                    Description = $"Duplicate function name detected: {functionName}.",
                    Code = fileContents,
                };
            }
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

    /// <summary>
    /// Extracts functions from the provided file contents.
    /// </summary>
    /// <param name="fileContents">The contents of the file to extract functions from.</param>
    /// <returns>A list of function strings extracted from the file contents.</returns>
    internal static List<string> ExtractFunctions(string fileContents)
    {
        var functions = new List<string>();
        var functionPattern = @"(public|private|protected|internal|static|async)?\s*(void|int|string|bool|Task|List<.*?>|Dictionary<.*?>|IEnumerable<.*?>|IList<.*?>|IDictionary<.*?>|ICollection<.*?>|IReadOnlyList<.*?>|IReadOnlyDictionary<.*?>|IReadOnlyCollection<.*?>|IQueryable<.*?>|IAsyncEnumerable<.*?>|IAsyncEnumerator<.*?>|IAsyncDisposable<.*?>)?\s+\w+\s*\(.*?\)\s*{";
        var matches = Regex.Matches(fileContents, functionPattern, RegexOptions.Singleline);

        foreach (Match match in matches)
        {
            var startIndex = match.Index;
            var braceCount = 0;
            var endIndex = startIndex;

            // Handle nested braces when extracting function bodies.
            for (int i = startIndex; i < fileContents.Length; i++)
            {
                if (fileContents[i] == '{')
                {
                    braceCount++;
                }
                else if (fileContents[i] == '}')
                {
                    braceCount--;
                    if (braceCount == 0)
                    {
                        endIndex = i;
                        break;
                    }
                }
            }

            if (braceCount == 0)
            {
                var function = fileContents.Substring(startIndex, endIndex - startIndex + 1);
                functions.Add(function);
            }
        }

        return functions;
    }

    /// <summary>
    /// Extracts the content of a function, excluding the function signature and braces.
    /// </summary>
    /// <param name="function">The function string to extract content from.</param>
    /// <returns>The content of the function as a string.</returns>
    /// <exception cref="ArgumentException">Thrown when the function string does not contain valid '{' and '}' characters.</exception>
    internal static string ExtractFunctionContent(string function)
    {
        var startIndex = function.IndexOf('{') + 1;
        var endIndex = function.LastIndexOf('}');

        if (startIndex == 0 || endIndex == -1 || endIndex < startIndex)
        {
            throw new ArgumentException("The function string does not contain valid '{' and '}' characters.");
        }

        var content = function.Substring(startIndex, endIndex - startIndex).Trim();

        // Normalize indentation.
        var lines = content.Split([Environment.NewLine], StringSplitOptions.None);
        var trimmedLines = lines.Select(line => line.TrimStart());
        return string.Join(Environment.NewLine, trimmedLines);
    }

    /// <summary>
    /// Extracts the name of a function from the function string.
    /// </summary>
    /// <param name="function">The function string to extract the name from.</param>
    /// <returns>The name of the function as a string.</returns>
    internal static string ExtractFunctionName(string function)
    {
        var match = Regex.Match(function, @"\b\w+\s*\(.*?\)\s*{");
        if (match.Success)
        {
            var functionSignature = match.Value;
            var nameMatch = Regex.Match(functionSignature, @"\b\w+\b");
            if (nameMatch.Success)
            {
                return nameMatch.Value;
            }
        }

        throw new ArgumentException("The function string does not contain a valid function name.");
    }

    /// <summary>
    /// Calculates the Jaccard similarity between two strings.
    /// </summary>
    /// <param name="content1">The first string to compare.</param>
    /// <param name="content2">The second string to compare.</param>
    /// <returns>The Jaccard similarity coefficient between the two strings.</returns>
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