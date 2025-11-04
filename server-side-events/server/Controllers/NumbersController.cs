using Microsoft.AspNetCore.Mvc;
using ServerSideEventsApi.Services;
using System.Text;
using System.Text.Json;

namespace ServerSideEventsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NumbersController : ControllerBase
{
    private readonly NumbersService _numbersService;
    private readonly ILogger<NumbersController> _logger;

    public NumbersController(NumbersService numbersService, ILogger<NumbersController> logger)
    {
        _numbersService = numbersService;
        _logger = logger;
    }

    [HttpGet("current")]
    public IActionResult GetCurrentNumbers()
    {
        var numbers = _numbersService.GetCurrentNumbers();
        return Ok(numbers);
    }

    [HttpGet("stream")]
    public async Task StreamNumbers(CancellationToken cancellationToken)
    {
        Response.Headers.Append("Content-Type", "text/event-stream");
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        var channel = _numbersService.Subscribe();

        try
        {
            await foreach (var numbers in channel.Reader.ReadAllAsync(cancellationToken))
            {
                var json = JsonSerializer.Serialize(numbers);
                var data = $"data: {json}\n\n";
                var bytes = Encoding.UTF8.GetBytes(data);
                
                await Response.Body.WriteAsync(bytes, cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Client disconnected from numbers stream");
        }
        finally
        {
            _numbersService.Unsubscribe(channel);
        }
    }
}
