using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using Server.ServiceBus.Events;
using Server.ServiceBus.Publisher;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeesController(IUnitOfWork unitOfWork, IEventBusPublisher eventBus) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<Employee>> Get()
    {
        await eventBus.PublishAsync(new LogEvent
        {
            Message = "GET /employees",
            Timestamp = DateTime.UtcNow
        });

        // Question: What is wrong here?
        return await unitOfWork.Employees.Query.OrderBy(e => e.Id).ToListAsync();
    }
}
