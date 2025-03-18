namespace CodeSmellDetection;

using System.Collections.Generic;
using System.Threading.Tasks;
using CodeSmellDetection.Models;

/// <summary>
/// Interface for detecting code smells in a codebase.
/// </summary>
public interface ICodeSmellDetectionService
{
    /// <summary>
    /// Asynchronously detects code smells in the codebase.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of detected code smells.</returns>
    Task<List<CodeSmell>> DetectAsync();

    /// <summary>
    /// Asynchronously detects code smells in a specific file.
    /// </summary>
    /// <param name="fileName">The name of the file to analyze.</param>
    /// <param name="fileContents">The contents of the file to analyze.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of detected code smells.</returns>
    Task<List<CodeSmell>> DetectAsync(string fileName, string fileContents);
}