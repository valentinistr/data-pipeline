using Core.Data;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeesController(IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<Employee>>> Get(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 10)
    {
        if (skip < 0 || take <= 0)
        {
            return BadRequest("skip must be >= 0 and take must be > 0.");
        }

        var query = unitOfWork.Employees.Query.OrderBy(e => e.Id);
        var totalCount = await query.CountAsync();
        var items = await query.Skip(skip).Take(take).ToListAsync();

        return new PagedResult<Employee>
        {
            Items = items,
            TotalCount = totalCount,
        };
    }
}
