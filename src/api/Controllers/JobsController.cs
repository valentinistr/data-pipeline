using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class JobsController(IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<Job>> Get()
    {
        return await unitOfWork.Jobs.Query.OrderBy(j => j.Id).ToListAsync();
    }
}
