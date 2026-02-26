using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using SecureAssist.Infrastructure.AI;
using Xunit;
using DotNetEnv;

namespace SecureAssist.E2ETests;

public class TogetherAiE2ETests
{
    private readonly TogetherAiService _aiService;

    public TogetherAiE2ETests()
    {
        // Load .env from project root
        // Assuming the test runs from tests/SecureAssist.E2ETests/bin/Debug/net8.0
        // We might need to adjust the path to find the .env at e:\SecureAssist\.env
        Env.TraversePath().Load();
        
        var httpClient = new HttpClient();
        _aiService = new TogetherAiService(httpClient);
    }

    [Fact]
    public async Task ProcessPromptAsync_ShouldReturnValidResponse_FromTogetherAi()
    {
        // Arrange
        var testPrompt = "Translate 'Hello, how are you?' to Spanish. Respond only with the translation.";

        // Act
        var response = await _aiService.ProcessPromptAsync(testPrompt);

        // Assert
        response.Should().NotBeNullOrEmpty();
        response.ToLower().Should().Contain("hola");
        
        // Log results for the report
        Console.WriteLine($"E2E Test Success.");
        Console.WriteLine($"Prompt: {testPrompt}");
        Console.WriteLine($"Response: {response}");
    }
}
