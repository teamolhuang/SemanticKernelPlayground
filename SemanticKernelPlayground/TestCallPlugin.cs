using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SemanticKernelPlayground;

public class TestCallPlugin
{
    [KernelFunction("get_guid")]
    [Description("Get a new guid.")]
    public async Task<string> TestCallAsync()
    {
        string guid = Guid.NewGuid().ToString();
        Console.WriteLine($"TestCallAsync --> {guid}");
        return guid;
    }
}