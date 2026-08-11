using ExcelAnalytics.Domain.Entities;

namespace ExcelAnalytics.Application.Interfaces;

public interface ICountryRepository
{
    Task<List<Country>> GetAllAsync();

    Task<Country?> GetByIdAsync(Guid id);

    Task AddAsync(Country country);

    Task UpdateAsync(Country country);

    Task DeleteAsync(Guid id);
}