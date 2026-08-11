using ExcelAnalytics.Application.DTOs.RateConfiguration;

namespace ExcelAnalytics.Application.Interfaces;

public interface IRateConfigurationService
{
    Task<List<RateConfigurationDto>> GetAllAsync();
    Task<RateConfigurationDto?> GetByIdAsync(Guid id);
    Task CreateAsync(CreateRateConfigurationDto dto);
    Task UpdateAsync(Guid id, UpdateRateConfigurationDto dto);
    Task DeleteAsync(Guid id);
}