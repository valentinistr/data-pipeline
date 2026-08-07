using Api.Extensions;
using Api.Services;
using Core.Data;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DataImportsController(
    IUnitOfWork unitOfWork,
    IDataManagementService dataManagementService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<DataImport>>> Get(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 10)
    {
        if (skip < 0 || take <= 0)
        {
            return BadRequest("skip must be >= 0 and take must be > 0.");
        }

        var query = unitOfWork.DataImports.Query.OrderBy(d => d.Id);
        var totalCount = await query.CountAsync();
        var items = await query.Skip(skip).Take(take).ToListAsync();

        return new PagedResult<DataImport>
        {
            Items = items,
            TotalCount = totalCount,
        };
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

        try
        {
            await dataManagementService.UploadAsync(jobsFile, employeesFile, cancellationToken);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        return Ok();
    }
}
