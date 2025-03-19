namespace CodeRefactorService;

using System;
using System.Threading.Tasks;
using Azure;
using Azure.AI.Inference;
using CodeSmellDetection.Models;
using Microsoft.Extensions.Configuration;

internal sealed class CodeRefactorService : ICodeRefactorService
{
    private readonly IConfiguration configuration;
    private readonly ChatCompletionsClient chatClient;

    public CodeRefactorService(IConfiguration configuration)
    {
        this.configuration = configuration;
        var endpoint = this.configuration["AzureOpenAI:Endpoint"]
               ?? throw new InvalidOperationException("Missing configuration: AzureOpenAI:Endpoint. See the README for details.");
        var key = this.configuration["AzureOpenAI:Key"]
                       ?? throw new InvalidOperationException("Missing configuration: AzureOpenAI:Key. See the README for details.");

        this.chatClient = new(new Uri(endpoint), new AzureKeyCredential(key));
    }

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