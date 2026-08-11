using ExcelAnalytics.Application.DTOs.Currency;
using ExcelAnalytics.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExcelAnalytics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CurrencyController : ControllerBase
{
    private readonly ICurrencyService _service;

    public CurrencyController(ICurrencyService service)
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
        var currency = await _service.GetByIdAsync(id);

        if (currency == null)
            return NotFound();

        return Ok(currency);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCurrencyDto dto)
    {
        await _service.CreateAsync(dto);
        return Ok("Currency created successfully.");
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateCurrencyDto dto)
    {
        await _service.UpdateAsync(id, dto);
        return Ok("Currency updated successfully.");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return Ok("Currency deleted successfully.");
    }
}