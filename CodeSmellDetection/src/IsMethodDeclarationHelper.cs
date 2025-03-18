namespace CodeSmellDetection;

using System;

internal class IsMethodDeclarationHelper
{
    internal static bool IsMethodDeclaration(string line)
    {
        return (line.StartsWith("public", StringComparison.Ordinal) ||
                line.StartsWith("private", StringComparison.Ordinal) ||
                line.StartsWith("protected", StringComparison.Ordinal) ||
                line.StartsWith("internal", StringComparison.Ordinal)) &&
                line.Contains("(") && line.Contains(")") &&
                !line.Contains("class");
    }
}