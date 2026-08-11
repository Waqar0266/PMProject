using ExcelAnalytics.Application.DTOs.Country;

namespace ExcelAnalytics.Application.Interfaces;

public interface ICountryService
{
    Task<List<CountryDto>> GetAllAsync();

    Task<CountryDto?> GetByIdAsync(Guid id);

    Task CreateAsync(CreateCountryDto dto);

    Task UpdateAsync(Guid id, UpdateCountryDto dto);

    Task DeleteAsync(Guid id);
}