using Microsoft.AspNetCore.Mvc;
using ServerSideEventsApi.Models;
using ServerSideEventsApi.Services;
using System.Text;
using System.Text.Json;

namespace ServerSideEventsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ChatService _chatService;
    private readonly ILogger<ChatController> _logger;

    public ChatController(ChatService chatService, ILogger<ChatController> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    [HttpPost("message")]
    public IActionResult PostMessage([FromBody] ChatMessage message)
    {
        if (string.IsNullOrWhiteSpace(message.UserName) || string.IsNullOrWhiteSpace(message.Message))
        {
            return BadRequest("UserName and Message are required");
        }

        message.Timestamp = DateTime.UtcNow;
        _chatService.AddMessage(message);
        
        return Ok(new { success = true });
    }

    [HttpGet("history")]
    public IActionResult GetHistory()
    {
        var history = _chatService.GetMessageHistory();
        return Ok(history);
    }

    [HttpGet("stream")]
    public async Task StreamMessages(CancellationToken cancellationToken)
    {
        Response.Headers.Append("Content-Type", "text/event-stream");
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        var channel = _chatService.Subscribe();

        try
        {
            await foreach (var message in channel.Reader.ReadAllAsync(cancellationToken))
            {
                var json = JsonSerializer.Serialize(message);
                var data = $"data: {json}\n\n";
                var bytes = Encoding.UTF8.GetBytes(data);
                
                await Response.Body.WriteAsync(bytes, cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Client disconnected from chat stream");
        }
        finally
        {
            _chatService.Unsubscribe(channel);
        }
    }
}
