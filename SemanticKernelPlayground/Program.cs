using System.Diagnostics.CodeAnalysis;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OllamaSharp;
using OllamaSharp.Models;
using OllamaSharp.Models.Chat;

namespace SemanticKernelPlayground;

static class Program
{
    [Experimental("SKEXP0070")]
    private static async Task Main()
    {
        IKernelBuilder builder = Kernel.CreateBuilder()
            .AddOllamaChatCompletion("mistral-nemo:latest", new Uri("http://localhost:11434"));

        builder.Plugins.AddFromType<TestCallPlugin>("Guid");
        
        Kernel kernel = builder.Build();
        
        ChatHistory context = new();
        
        string? input = "";
        while (string.IsNullOrWhiteSpace(input))
        {
            Console.Write("請輸入想問 ollama 的問題：");
            input = Console.ReadLine();
        }
        
        context.AddUserMessage(input);

        IChatCompletionService chat = kernel.GetRequiredService<IChatCompletionService>();
        
        // limitations:
        // 1. deepseek doesn't support tools, so we're using mistral-nemo here.
        // 2. ollama currently only support tools with sync mode.
        //
        // To use tools in deepseek, we might need to abandon the abstraction completely,
        // and simply have a way to exchange JSONs with deepseek, manually manipulating the context.
        
        IReadOnlyList<ChatMessageContent> replies = await chat.GetChatMessageContentsAsync(context, 
            executionSettings: new OllamaPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            },
            kernel: kernel);
        
        Console.WriteLine("\n---------------------------------------\n");
        
        foreach (ChatMessageContent reply in replies)
        {
            Console.Write(reply.ToString());
        }
    }
}
