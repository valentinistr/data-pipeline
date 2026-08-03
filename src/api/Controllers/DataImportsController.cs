using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using Server.ServiceBus.Events;
using Server.ServiceBus.Publisher;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DataImportsController(IUnitOfWork unitOfWork, IEventBusPublisher eventBus) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<DataImport>> Get()
    {
        await eventBus.PublishAsync(new LogEvent
        {
            Message = "GET /dataimports",
            Timestamp = DateTime.UtcNow
        });

        return await unitOfWork.DataImports.Query.OrderBy(d => d.Id).ToListAsync();
    }
}
