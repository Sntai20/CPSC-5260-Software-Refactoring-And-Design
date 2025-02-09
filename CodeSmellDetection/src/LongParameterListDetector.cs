namespace CodeSmellDetection;

using System.Linq;
using CodeSmellDetection.Options;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// Detects methods with long parameter lists in C# source code.
/// </summary>
internal class LongParameterListDetector(
    IOptions<LongParameterListDetectorOptions> options,
    ILogger<LongParameterListDetector> logger)
{
    private readonly IOptions<LongParameterListDetectorOptions> options = options;
    private readonly ILogger<LongParameterListDetector> logger = logger;

    /// <summary>
    /// Detects methods with parameter lists longer than the specified threshold in the provided C# source code.
    /// </summary>
    /// <param name="fileContents">The contents of the C# source file to analyze.</param>
    public void DetectLongParameterLists(string fileContents)
    {
        var tree = CSharpSyntaxTree.ParseText(fileContents);
        var root = tree.GetRoot();
        var parameterThreshold = this.options.Value.ParameterThreshold;

        var methodsWithLongParameterLists = root.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Where(m => m.ParameterList.Parameters.Count > parameterThreshold);

        foreach (var method in methodsWithLongParameterLists)
        {
            this.logger.LogInformation($"Method '{method.Identifier}' has a long parameter list with {method.ParameterList.Parameters.Count} parameters.");
        }
    }
}