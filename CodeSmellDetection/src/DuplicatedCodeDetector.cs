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
    private static readonly char[] Separator = [' ', '\t', '(', ')', '{', '}', ';', ','];
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
        var functionSets = new List<HashSet<string>>();

        foreach (var function in functions)
        {
            var words = function.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
            functionSets.Add(new HashSet<string>(words));
        }

        for (int i = 0; i < functionSets.Count; i++)
        {
            var currentSet = functionSets[i];
            for (int j = i + 1; j < functionSets.Count; j++)
            {
                var comparisonSet = functionSets[j];
                var intersectionCount = 0;
                var unionCount = currentSet.Count;

                foreach (var word in comparisonSet)
                {
                    if (currentSet.Contains(word))
                    {
                        intersectionCount++;
                    }
                    else
                    {
                        unionCount++;
                    }
                }

                double jaccardSimilarity = (double)intersectionCount / unionCount;

                if (jaccardSimilarity >= this.options.Value.JaccardThreshold)
                {
                    this.logger.LogWarning($"Duplicated code detected between functions {i + 1} and {j + 1} with Jaccard similarity {jaccardSimilarity}");
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
}