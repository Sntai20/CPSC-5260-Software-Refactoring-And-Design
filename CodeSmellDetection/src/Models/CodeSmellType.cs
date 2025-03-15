namespace CodeSmellDetection.Models;

/// <summary>
/// Represents the different types of code smells that can be detected.
/// </summary>
public enum CodeSmellType
{
    /// <summary>
    /// Indicates duplicated code in the codebase.
    /// </summary>
    DuplicatedCode,

    /// <summary>
    /// Indicates a method that is excessively long.
    /// </summary>
    LongMethod,

    /// <summary>
    /// Indicates a method with a long list of parameters.
    /// </summary>
    LongParameterLists,
}