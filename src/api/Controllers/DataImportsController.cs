using Api.Extensions;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using Server.ServiceBus.Events;
using Server.ServiceBus.Publisher;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DataImportsController(
    IUnitOfWork unitOfWork,
    IEventBusPublisher eventBus,
    IDataManagementService dataManagementService) : ControllerBase
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

    [HttpPost("upload")]
    [RequestSizeLimit(50_000_000)]
    public async Task<IActionResult> Upload(IFormFile? jobs, IFormFile? employees, CancellationToken cancellationToken)
    {
        if (jobs is null && employees is null)
        {
            return BadRequest("At least one file is required.");
        }

        await using var jobsFile = jobs?.ToUploadedFile();
        await using var employeesFile = employees?.ToUploadedFile();

        string folderPath;
        try
        {
            folderPath = await dataManagementService.UploadAsync(jobsFile, employeesFile, cancellationToken);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        await eventBus.PublishAsync(new LogEvent
        {
            Message = $"POST /dataimports/upload path={folderPath} jobs={jobs?.FileName ?? "(none)"} employees={employees?.FileName ?? "(none)"}",
            Timestamp = DateTime.UtcNow
        });

        return Ok();
    }
}