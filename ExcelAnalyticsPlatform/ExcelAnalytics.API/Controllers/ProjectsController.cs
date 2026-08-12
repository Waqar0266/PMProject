using ExcelAnalytics.Application.DTOs.Project;
using ExcelAnalytics.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExcelAnalytics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _service;

    public ProjectsController(IProjectService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var project = await _service.GetByIdAsync(id);

        if (project == null)
            return NotFound();

        return Ok(project);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProjectDto dto)
    {
        try
        {
            await _service.CreateAsync(dto);
            return Ok("Project created successfully.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateProjectDto dto)
    {
        try
        {
            await _service.UpdateAsync(id, dto);
            return Ok("Project updated successfully.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return Ok("Project deleted successfully.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
