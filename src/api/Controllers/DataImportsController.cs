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
    public async Task<IEnumerable<DataImport>> Get()
    {
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
