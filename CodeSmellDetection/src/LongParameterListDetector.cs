namespace CodeSmellDetection;

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal class LongParameterListDetector
{
    private const int ParameterThreshold = 3;

    public void DetectLongParameterLists(string fileContents)
    {
        var tree = CSharpSyntaxTree.ParseText(fileContents);
        var root = tree.GetRoot();

        var methodsWithLongParameterLists = root.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Where(m => m.ParameterList.Parameters.Count > ParameterThreshold);

        foreach (var method in methodsWithLongParameterLists)
        {
            Console.WriteLine($"Method '{method.Identifier}' has a long parameter list with {method.ParameterList.Parameters.Count} parameters.");
        }
    }
}