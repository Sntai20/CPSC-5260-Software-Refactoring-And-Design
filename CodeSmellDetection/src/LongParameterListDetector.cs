﻿namespace CodeSmellDetection;

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

internal class LongParameterListDetector(ILogger<LongParameterListDetector> logger)
{
    private const int ParameterThreshold = 3;
    private readonly ILogger<LongParameterListDetector> logger = logger;

    public void DetectLongParameterLists(string fileContents)
    {
        var tree = CSharpSyntaxTree.ParseText(fileContents);
        var root = tree.GetRoot();

        var methodsWithLongParameterLists = root.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Where(m => m.ParameterList.Parameters.Count > ParameterThreshold);

        foreach (var method in methodsWithLongParameterLists)
        {
            this.logger.LogInformation($"Method '{method.Identifier}' has a long parameter list with {method.ParameterList.Parameters.Count} parameters.");
        }
    }
}