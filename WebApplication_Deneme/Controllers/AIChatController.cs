using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace WebApplication_Deneme.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIChatController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _model;

        public AIChatController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _model = config["OpenAI:Model"];
        }

        public class ChatRequest { public string Prompt { get; set; } }

        // Chat Completions API models
        private class Message { public string Role { get; set; } public string Content { get; set; } }
        private class Choice { public Message Message { get; set; } }
        private class ChatResponse { public Choice[] Choices { get; set; } }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Prompt))
                return BadRequest(new { error = "Prompt boş olamaz." });

            var client = _httpClientFactory.CreateClient("OpenAI");

            var payload = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful assistant." },
                    new { role = "user",   content = req.Prompt }
                }
            };

            HttpResponseMessage response;
            try
            {
                response = await client.PostAsync(
                "chat/completions",
                new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
               );
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("OpenAI request failed: " + ex);
                return StatusCode(502, new { error = "OpenAI servisine ulaşılamadı." });
            }

            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                Console.Error.WriteLine($"OpenAI error {response.StatusCode}: {content}");
                return StatusCode((int)response.StatusCode, new { error = content });
            }

            var chat = JsonSerializer.Deserialize<ChatResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var answer = chat?.Choices?.FirstOrDefault()?.Message?.Content?.Trim();
            if (string.IsNullOrEmpty(answer))
                return StatusCode(500, new { error = "Cevap alınamadı." });

            return Ok(new { answer });
        }
    }
}