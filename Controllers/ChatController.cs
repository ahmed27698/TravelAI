using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using TravelAI.Data;
using TravelAI.Models;

namespace TravelAI.Controllers;

[Authorize]
public class ChatController : Controller
{
    private readonly TravelAIDbContext _db;
    private readonly IConfiguration _config;
    private static readonly HttpClient _http = new();

    public ChatController(TravelAIDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [HttpGet]
    public async Task<IActionResult> History()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var messages = await _db.ChatMessages
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.SentAt)
            .Take(50)
            .ToListAsync();
        return Json(messages.OrderBy(m => m.SentAt));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("chat")]
    public async Task<IActionResult> Send([FromBody] string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return BadRequest();

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var userMsg = new ChatMessage { UserId = userId, Role = "user", Content = content };
        _db.ChatMessages.Add(userMsg);

        var recentHistory = await _db.ChatMessages
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.SentAt)
            .Take(20)
            .ToListAsync();
        recentHistory.Reverse();

        string aiResponse;
        try
        {
            var apiKey = _config["GroqApiKey"]
                ?? throw new InvalidOperationException("GroqApiKey not configured.");

            var messages = new List<object>
            {
                new { role = "system", content = "You are TravelAI, a smart and friendly AI travel assistant. " +
                    "Help users plan trips, discover destinations, find hotels and flights, " +
                    "and answer any travel-related questions. Be concise, enthusiastic, and knowledgeable. " +
                    "Use bullet points and short paragraphs. Keep responses under 200 words unless detail is truly needed." }
            };

            foreach (var msg in recentHistory)
                messages.Add(new { role = msg.Role, content = msg.Content });

            messages.Add(new { role = "user", content });

            var payload = JsonSerializer.Serialize(new
            {
                model = "llama-3.3-70b-versatile",
                messages,
                max_tokens = 1024,
                temperature = 0.7
            });

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await _http.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            aiResponse = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString()
                ?? "I'm here to help you plan your next adventure!";
        }
        catch (Exception ex)
        {
            aiResponse = $"I'm your AI travel assistant! Connection issue: {ex.Message.Split('\n')[0]}";
        }

        var aiMsg = new ChatMessage { UserId = userId, Role = "assistant", Content = aiResponse };
        _db.ChatMessages.Add(aiMsg);
        await _db.SaveChangesAsync();

        return Json(new { role = aiMsg.Role, content = aiMsg.Content, sentAt = aiMsg.SentAt });
    }
}
