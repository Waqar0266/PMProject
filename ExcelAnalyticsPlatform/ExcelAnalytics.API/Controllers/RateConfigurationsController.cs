using ExcelAnalytics.Application.DTOs.RateConfiguration;
using ExcelAnalytics.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExcelAnalytics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RateConfigurationsController : ControllerBase
{
    private readonly IRateConfigurationService _service;

    public RateConfigurationsController(IRateConfigurationService service)
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
        var rate = await _service.GetByIdAsync(id);

        if (rate == null)
            return NotFound();

        return Ok(rate);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRateConfigurationDto dto)
    {
        await _service.CreateAsync(dto);
        return Ok("Rate configuration created successfully.");
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateRateConfigurationDto dto)
    {
        await _service.UpdateAsync(id, dto);
        return Ok("Rate configuration updated successfully.");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return Ok("Rate configuration deleted successfully.");
    }
}