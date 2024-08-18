using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using AI_Chat_API.Models;

namespace AI_Chat_API.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();
        }

        public async Task<List<string>> FetchModelsAsync(string apiUrl, string apiKey)
        {
            var models = new List<string>();

            try
            {
                var requestUrl = $"{apiUrl}/v1/models";
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var response = await _httpClient.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
                    {
                        JsonElement root = doc.RootElement;

                        if (root.TryGetProperty("data", out JsonElement dataElement) && dataElement.ValueKind == JsonValueKind.Array)
                        {
                            foreach (JsonElement model in dataElement.EnumerateArray())
                            {
                                if (model.TryGetProperty("id", out JsonElement idElement))
                                {
                                    models.Add(idElement.GetString());
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Log the error response
                    Console.WriteLine($"Failed to fetch models: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                // Handle exception (logging, rethrowing, etc.)
                Console.WriteLine($"Exception in FetchModelsAsync: {ex.Message}");
            }

            return models;
        }

        public async Task<string> SendMessageAsync(string message, string llmDirections, string model, APIConfiguration config, float temperature = 0.7f)
        {
            try
            {
                var payload = new
                {
                    model = model,
                    messages = new[]
                    {
                        new { role = "system", content = llmDirections },
                        new { role = "user", content = message }
                    },
                    temperature = temperature
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.ApiKey);

                var response = await _httpClient.PostAsync($"{config.Url}/v1/chat/completions", content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<JsonDocument>(responseBody);

                return jsonResponse.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
            }
            catch (Exception ex)
            {
                // Handle exception (logging, rethrowing, etc.)
                Console.WriteLine($"Exception in SendMessageAsync: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }
    }
}
