using ExcelAnalytics.Domain.Entities;

namespace ExcelAnalytics.Application.Interfaces;

public interface ICurrencyRepository
{
    Task<List<Currency>> GetAllAsync();

    Task<Currency?> GetByIdAsync(Guid id);

    Task AddAsync(Currency currency);

    Task UpdateAsync(Currency currency);

    Task DeleteAsync(Guid id);
}