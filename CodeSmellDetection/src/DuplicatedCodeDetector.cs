namespace CodeSmellDetection;

using System;
using System.Collections.Generic;
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
    public void DetectDuplicatedCode(string fileContents)
    {
        var lineSets = new List<HashSet<string>>();

        foreach (var line in fileContents.Split(Environment.NewLine))
        {
            var words = line.Split(
                Separator,
                StringSplitOptions.RemoveEmptyEntries);
            lineSets.Add(new HashSet<string>(words));
        }

        for (int i = 0; i < lineSets.Count; i++)
        {
            for (int j = i + 1; j < lineSets.Count; j++)
            {
                var intersection = new HashSet<string>(lineSets[i]);
                intersection.IntersectWith(lineSets[j]);

                var union = new HashSet<string>(lineSets[i]);
                union.UnionWith(lineSets[j]);

                double jaccardSimilarity = (double)intersection.Count / union.Count;

                if (jaccardSimilarity >= this.options.Value.JaccardThreshold)
                {
                    this.logger.LogWarning($"Duplicated code detected between lines {i + 1} and {j + 1} with Jaccard similarity {jaccardSimilarity}");
                }
            }
        }
    }
}