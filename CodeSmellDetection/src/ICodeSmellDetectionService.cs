namespace CodeSmellDetection;

using System.Collections.Generic;
using CodeSmellDetection.Models;

/// <summary>
/// Interface for detecting code smells in a codebase.
/// </summary>
public interface ICodeSmellDetectionService
{
    /// <summary>
    /// Detects code smells in the codebase.
    /// </summary>
    /// <returns>A list of detected code smells.</returns>
    List<CodeSmell> Detect();
}