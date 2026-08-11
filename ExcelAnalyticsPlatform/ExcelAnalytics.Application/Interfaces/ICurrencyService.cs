using ExcelAnalytics.Application.DTOs.Currency;

namespace ExcelAnalytics.Application.Interfaces;

public interface ICurrencyService
{
    Task<List<CurrencyDto>> GetAllAsync();

    Task<CurrencyDto?> GetByIdAsync(Guid id);

    Task CreateAsync(CreateCurrencyDto dto);

    Task UpdateAsync(Guid id, UpdateCurrencyDto dto);

    Task DeleteAsync(Guid id);
}