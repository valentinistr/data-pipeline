using Microsoft.AspNetCore.Mvc;
using Server.ServiceBus.Events;
using Server.ServiceBus.Publisher;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController(IEventBusPublisher eventBus) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        await eventBus.PublishAsync(new LogEvent
        {
            Message = "GET /health",
            Timestamp = DateTime.UtcNow
        });

        return Content("Healthy", "text/plain");
    }
}
