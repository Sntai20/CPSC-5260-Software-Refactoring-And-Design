namespace CodeSmellDetection;

using Azure;
using Azure.AI.Inference;
using CodeSmellDetection.Detections;
using CodeSmellDetection.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

/// <inheritdoc />
internal class CodeSmellDetectionService
    : ICodeSmellDetectionService
{
    private readonly StructuralDuplicateCode structuralDuplicateCodeDetection;
    private readonly LongMethod longMethodDetection;
    private readonly LongParameterList longParameterListDetection;
    private readonly ILogger<CodeSmellDetectionService> logger;
    private readonly IConfiguration configuration;
    private readonly ChatCompletionsClient chatClient;

    public CodeSmellDetectionService(
        StructuralDuplicateCode structuralDuplicateCodeDetection,
        LongMethod longMethodDetection,
        LongParameterList longParameterListDetection,
        ILogger<CodeSmellDetectionService> logger,
        IConfiguration configuration)
    {
        this.structuralDuplicateCodeDetection = structuralDuplicateCodeDetection;
        this.longMethodDetection = longMethodDetection;
        this.longParameterListDetection = longParameterListDetection;
        this.logger = logger;
        this.configuration = configuration;

        var endpoint = this.configuration["AzureOpenAI:Endpoint"]
               ?? throw new InvalidOperationException("Missing configuration: AzureOpenAI:Endpoint. See the README for details.");
        var key = this.configuration["AzureOpenAI:Key"]
                       ?? throw new InvalidOperationException("Missing configuration: AzureOpenAI:Key. See the README for details.");

        this.chatClient = new(new Uri(endpoint), new AzureKeyCredential(key));
    }

    /// <inheritdoc />
    public async Task<List<CodeSmell>> DetectAsync()
    {
        string? pathToCodeFile = this.configuration["PathToCodeFile"]
                                 ?? throw new InvalidOperationException("Path to fileContents file not found in configuration.");
        string fileContents = await File.ReadAllTextAsync(pathToCodeFile);
        string fileName = Path.GetFileName(pathToCodeFile);

        return await this.DetectAsync(fileName, fileContents);
    }

    /// <inheritdoc />
    public async Task<List<CodeSmell>> DetectAsync(string fileName, string fileContents)
    {
        var codeSmells = new List<CodeSmell>();

        List<CodeSmell>? structuralDuplicateCodeSmells = await Task.Run(() => this.structuralDuplicateCodeDetection.Detect(fileContents));
        codeSmells.AddRange(structuralDuplicateCodeSmells);

        List<CodeSmell>? longMethodSmells = await Task.Run(() => this.longMethodDetection.Detect(fileContents));
        codeSmells.AddRange(longMethodSmells);

        List<CodeSmell>? longParameterListSmells = await Task.Run(() => this.longParameterListDetection.Detect(fileContents));
        codeSmells.AddRange(longParameterListSmells);

        this.logger.LogInformation($"Detected {codeSmells.Count} code smells in the provided code file {fileName}.");

        foreach (var smell in codeSmells)
        {
            smell.FileName = fileName;
        }

        return codeSmells;
    }

    /// <inheritdoc />
    public Task<CodeSmell> RefactorAsync()
    {
        return this.RefactorAsync(new CodeSmell
        {
            Code = "Console.WriteLine(\"Hello, World!\");",
            SmellCodeRecommendation = "Console.WriteLine(\"Hello, World!\");",
        });
    }

    public async Task<CodeSmell> RefactorAsync(CodeSmell codeSmell)
    {
        var requestOptions = new ChatCompletionsOptions()
        {
            Messages =
            {
                new ChatRequestSystemMessage("You are a helpful assistant."),
                new ChatRequestUserMessage("How many feet are in a mile?"),
            },
            Model = "gpt-4o-mini",
        };

        Response<ChatCompletions> response = await this.chatClient.CompleteAsync(requestOptions);
        Console.WriteLine(response.Value.Content);
        return new CodeSmell
        {
            Code = codeSmell.Code,
            SmellCodeRecommendation = response.Value.Content,
        };
    }
}