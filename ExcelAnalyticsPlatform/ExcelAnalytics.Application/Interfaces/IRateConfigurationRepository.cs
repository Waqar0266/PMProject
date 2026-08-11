using ExcelAnalytics.Domain.Entities;

namespace ExcelAnalytics.Application.Interfaces;

public interface IRateConfigurationRepository
{
    Task<List<RateConfiguration>> GetAllAsync();
    Task<RateConfiguration?> GetByIdAsync(Guid id);
    Task AddAsync(RateConfiguration rate);
    Task UpdateAsync(RateConfiguration rate);
    Task DeleteAsync(Guid id);
    Task<RateConfiguration?> GetByCountryYearMonthAsync(
    string countryCode,
    int year,
    int month);
}