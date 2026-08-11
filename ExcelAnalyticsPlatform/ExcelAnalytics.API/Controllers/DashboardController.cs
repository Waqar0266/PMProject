using ExcelAnalytics.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExcelAnalytics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IAllocationRepository _repository;

    public DashboardController(IAllocationRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var data = await _repository.GetAllAsync();

        return Ok(data);
    }
    [HttpGet("summary")]
    public async Task<IActionResult> Summary()
    {
        var data = await _repository.GetAllAsync();

        var result = new
        {
            TotalEmployees = data.Count,

            TotalProjects = data
                .Select(x => x.ProjectName)
                .Distinct()
                .Count(),

            TotalCountries = data
                .Select(x => x.AllocatedFor)
                .Distinct()
                .Count(),

            TotalAllocationDays = data.Sum(x => x.AllocationDays),

            TotalRevenue = data.Sum(x => x.RevenueUSD)
        };

        return Ok(result);
    }
    [HttpGet("country")]
    public async Task<IActionResult> Country()
    {
        var data = await _repository.GetAllAsync();

        var result = data
            .GroupBy(x => x.AllocatedFor)
            .Select(x => new
            {
                Country = x.Key,
                Employees = x.Count(),
                Revenue = x.Sum(y => y.RevenueUSD)
            });

        return Ok(result);
    }
    [HttpGet("project")]
    public async Task<IActionResult> Project()
    {
        var data = await _repository.GetAllAsync();

        var result = data
            .GroupBy(x => x.ProjectName)
            .Select(x => new
            {
                Project = x.Key,
                Employees = x.Count(),
                Revenue = x.Sum(y => y.RevenueUSD)
            });

        return Ok(result);
    }

}