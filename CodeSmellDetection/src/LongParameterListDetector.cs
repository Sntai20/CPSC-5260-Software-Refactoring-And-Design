namespace CodeSmellDetection;

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

/// <summary>
/// Detects methods with long parameter lists in C# source code.
/// </summary>
internal class LongParameterListDetector(ILogger<LongParameterListDetector> logger)
{
    private const int ParameterThreshold = 3;
    private readonly ILogger<LongParameterListDetector> logger = logger;

    /// <summary>
    /// Detects methods with parameter lists longer than the specified threshold in the provided C# source code.
    /// </summary>
    /// <param name="fileContents">The contents of the C# source file to analyze.</param>
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