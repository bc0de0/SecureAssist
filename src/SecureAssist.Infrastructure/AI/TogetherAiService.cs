using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using SecureAssist.Application.Interfaces;

namespace SecureAssist.Infrastructure.AI;

public class TogetherAiService : IAiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;

    public TogetherAiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = Environment.GetEnvironmentVariable("TOGETHER_AI_API_KEY") ?? string.Empty;
        _model = Environment.GetEnvironmentVariable("TOGETHER_AI_MODEL") ?? "meta-llama/Llama-3.3-70B-Instruct-Turbo";
    }

    public async Task<string> ProcessPromptAsync(string prompt)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            return "Together AI API Key is missing. Please check your .env file.";
        }

        var requestBody = new
        {
            model = _model,
            messages = new[]
            {
                new { role = "user", content = prompt }
            },
            max_tokens = 512,
            temperature = 0.7,
            top_p = 0.7,
            top_k = 50,
            repetition_penalty = 1,
            stop = new[] { "<|eot_id|>", "<|eom_id|>" }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.together.xyz/v1/chat/completions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        request.Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            return $"AI Error: {response.StatusCode} - {errorContent}";
        }

        var resultJson = await response.Content.ReadAsStringAsync();
        dynamic result = JsonConvert.DeserializeObject(resultJson);
        
        return result.choices[0].message.content;
    }
}
