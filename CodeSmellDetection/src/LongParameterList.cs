namespace CodeSmellDetection;

using System.Linq;
using CodeSmellDetection.Models;
using CodeSmellDetection.Options;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// Detects methods with long parameter lists in C# source code.
/// </summary>
internal class LongParameterList(
    IOptions<LongParameterListOptions> options,
    ILogger<LongParameterList> logger)
{
    private readonly IOptions<LongParameterListOptions> options = options;
    private readonly ILogger<LongParameterList> logger = logger;

    /// <summary>
    /// Detects methods with parameter lists longer than the specified threshold in the provided C# source code.
    /// </summary>
    /// <param name="fileContents">The contents of the C# source file to analyze.</param>
    /// <returns>A <see cref="CodeSmell"/> object representing the detected code smell, or null if no code smell is detected.</returns>
    public CodeSmell Detect(string fileContents)
    {
        var tree = CSharpSyntaxTree.ParseText(fileContents);
        var root = tree.GetRoot();
        var parameterThreshold = this.options.Value.ParameterThreshold;

        var methodsWithLongParameterLists = root.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Where(m => m.ParameterList.Parameters.Count > parameterThreshold);

        foreach (var method in methodsWithLongParameterLists)
        {
            this.logger.LogWarning($"Method '{method.Identifier}' has a long parameter list with {method.ParameterList.Parameters.Count} parameters.");
            return new CodeSmell
            {
                Type = CodeSmellType.LongParameterLists,
                Code = fileContents,
                Description = $"Method '{method.Identifier}' has a long parameter list with {method.ParameterList.Parameters.Count} parameters.",
                LineNumber = method.GetLocation().GetMappedLineSpan().StartLinePosition.Line + 1,
                StartLine = method.GetLocation().GetMappedLineSpan().StartLinePosition.Line + 1,
                EndLine = method.GetLocation().GetMappedLineSpan().EndLinePosition.Line + 1,
                SmellRecommendation = "Consider refactoring the method to reduce the number of parameters.",
                SmellCodeRecommendation = $"public void {method.Identifier}({string.Join(", ", method.ParameterList.Parameters)}) {{ }}",
            };
        }

        this.logger.LogInformation("Methods with long parameter lists not detected.");
        return null;
    }
}