namespace CodeSmellDetection;

using System;
using System.Collections.Generic;

internal class DuplicatedCodeDetector
{
    private const double JaccardThreshold = 0.75;
    private static readonly char[] Separator = [' ', '\t', '(', ')', '{', '}', ';', ','];

    public void DetectDuplicatedCode(string fileContents)
    {
        var lineSets = new List<HashSet<string>>();

        foreach (var line in fileContents.Split("\n"))
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

                if (jaccardSimilarity >= JaccardThreshold)
                {
                    Console.WriteLine($"Duplicated code detected between lines {i + 1} and {j + 1} with Jaccard similarity {jaccardSimilarity}");
                }
            }
        }
    }
}