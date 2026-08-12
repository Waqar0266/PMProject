using ExcelAnalytics.Application.DTOs.RateConfiguration;
using ExcelAnalytics.Application.Interfaces;
using ExcelAnalytics.Domain.Entities;

namespace ExcelAnalytics.Application.Services;

public class RateConfigurationService : IRateConfigurationService
{
    private readonly IRateConfigurationRepository _repository;
    private readonly IAllocationRepository _allocationRepository;

    public RateConfigurationService(
        IRateConfigurationRepository repository,
        IAllocationRepository allocationRepository)
    {
        _repository = repository;
        _allocationRepository = allocationRepository;
    }

    public async Task<List<RateConfigurationDto>> GetAllAsync()
    {
        var rates = await _repository.GetAllAsync();

        return rates.Select(x => new RateConfigurationDto
        {
            Id = x.Id,
            CountryId = x.CountryId,
            CountryName = x.Country.Name,
            ProjectId = x.ProjectId,
            ProjectName = x.Project.Name,
            CurrencyId = x.CurrencyId,
            CurrencyName = x.Currency.Name,
            CurrencyCode = x.Currency.Code,
            CurrencySymbol = x.Currency.Symbol,
            Year = x.Year,
            Month = x.Month,
            Rate = x.Rate
        }).ToList();
    }

    public async Task<RateConfigurationDto?> GetByIdAsync(Guid id)
    {
        var rate = await _repository.GetByIdAsync(id);

        if (rate == null)
            return null;

        return new RateConfigurationDto
        {
            Id = rate.Id,
            CountryId = rate.CountryId,
            CountryName = rate.Country.Name,
            ProjectId = rate.ProjectId,
            ProjectName = rate.Project.Name,
            CurrencyId = rate.CurrencyId,
            CurrencyName = rate.Currency.Name,
            CurrencyCode = rate.Currency.Code,
            CurrencySymbol = rate.Currency.Symbol,
            Year = rate.Year,
            Month = rate.Month,
            Rate = rate.Rate
        };
    }

    public async Task CreateAsync(CreateRateConfigurationDto dto)
    {
        var rate = new RateConfiguration
        {
            CountryId = dto.CountryId,
            ProjectId = dto.ProjectId,
            CurrencyId = dto.CurrencyId,
            Year = dto.Year,
            Month = dto.Month,
            Rate = dto.Rate,
            IsActive = true
        };

        await _repository.AddAsync(rate);
    }

    public async Task UpdateAsync(Guid id, UpdateRateConfigurationDto dto)
    {
        var rateConfiguration = await _repository.GetByIdAsync(id);

        if (rateConfiguration == null)
            throw new Exception("Rate configuration not found.");

        rateConfiguration.CountryId = dto.CountryId;
        rateConfiguration.ProjectId = dto.ProjectId;
        rateConfiguration.CurrencyId = dto.CurrencyId;
        rateConfiguration.Year = dto.Year;
        rateConfiguration.Month = dto.Month;
        rateConfiguration.Rate = dto.Rate;
        rateConfiguration.IsActive = dto.IsActive;

        await _repository.UpdateAsync(rateConfiguration);

        rateConfiguration = await _repository.GetByIdAsync(id);

        if (rateConfiguration == null)
            return;

        decimal exchangeRate = rateConfiguration.Currency.ExchangeRateToUSD;

        var allocations = await _allocationRepository.GetAllAsync();

        var records = allocations.Where(x =>
            x.AllocatedFor == rateConfiguration.Country.Code &&
            x.ProjectName.Equals(rateConfiguration.Project.Name, StringComparison.OrdinalIgnoreCase) &&
            x.Year == dto.Year &&
            x.Month == dto.Month)
            .ToList();

        foreach (var record in records)
        {
            record.Rate = dto.Rate;
            record.Revenue = record.FinalAllocationDays * dto.Rate;
            record.RevenueUSD = record.Revenue * exchangeRate;
        }

        await _allocationRepository.SaveAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }
}
