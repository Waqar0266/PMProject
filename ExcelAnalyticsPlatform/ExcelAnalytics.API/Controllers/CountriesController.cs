using ExcelAnalytics.Application.DTOs.Country;
using ExcelAnalytics.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExcelAnalytics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CountriesController : ControllerBase
{
    private readonly ICountryService _service;

    public CountriesController(ICountryService service)
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
        var country = await _service.GetByIdAsync(id);

        if (country == null)
            return NotFound();

        return Ok(country);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCountryDto dto)
    {
        await _service.CreateAsync(dto);

        return Ok("Country created successfully.");
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateCountryDto dto)
    {
        await _service.UpdateAsync(id, dto);

        return Ok("Country updated successfully.");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);

        return Ok("Country deleted successfully.");
    }
}