namespace CodeSmellDetection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

public class LongParameterListDetector
{
    private const int ParameterThreshold = 3;

    public void DetectLongParameterLists(string filePath)
    {
        var code = System.IO.File.ReadAllText(filePath);
        var tree = CSharpSyntaxTree.ParseText(code);
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