namespace CodeRefactorService;

using CodeSmellDetection.Models;

internal interface ICodeRefactorService
{
    /// <summary>
    /// Asynchronously refactors the code to address the specified code smell.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains refactored code smells.</returns>
    Task<CodeSmell> RefactorAsync();

    /// <summary>
    /// Asynchronously refactors the code to address the specified code smell.
    /// </summary>
    /// <param name="codeSmell">The code smell to refactor.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains refactored code smells.</returns>
    Task<CodeSmell> RefactorAsync(CodeSmell codeSmell);
}