using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using Server.ServiceBus.Events;
using Server.ServiceBus.Publisher;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class JobsController(IUnitOfWork unitOfWork, IEventBusPublisher eventBus) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<Job>> Get()
    {
        await eventBus.PublishAsync(new LogEvent
        {
            Message = "GET /jobs",
            Timestamp = DateTime.UtcNow
        });

        return await unitOfWork.Jobs.Query.OrderBy(j => j.Id).ToListAsync();
    }
}
