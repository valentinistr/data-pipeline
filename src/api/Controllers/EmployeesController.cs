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
    public async Task<IEnumerable<Employee>> Get()
    {
        // Question: What is wrong here?
        return await unitOfWork.Employees.Query.OrderBy(e => e.Id).ToListAsync();
    }
}
