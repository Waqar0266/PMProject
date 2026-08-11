using ExcelAnalytics.Application.DTOs.Country;
using ExcelAnalytics.Application.Interfaces;
using ExcelAnalytics.Domain.Entities;

namespace ExcelAnalytics.Application.Services;

public class CountryService : ICountryService
{
    private readonly ICountryRepository _repository;

    public CountryService(ICountryRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CountryDto>> GetAllAsync()
    {
        var countries = await _repository.GetAllAsync();

        return countries.Select(x => new CountryDto
        {
            Id = x.Id,
            Code = x.Code,
            Name = x.Name,
            CurrencyCode = x.CurrencyCode,
            IsActive = x.IsActive
        }).ToList();
    }

    public async Task<CountryDto?> GetByIdAsync(Guid id)
    {
        var country = await _repository.GetByIdAsync(id);

        if (country == null)
            return null;

        return new CountryDto
        {
            Id = country.Id,
            Code = country.Code,
            Name = country.Name,
            CurrencyCode = country.CurrencyCode,
            IsActive = country.IsActive
        };
    }

    public async Task CreateAsync(CreateCountryDto dto)
    {
        var country = new Country
        {
            Code = dto.Code,
            Name = dto.Name,
            CurrencyCode = dto.CurrencyCode
        };

        await _repository.AddAsync(country);
    }

    public async Task UpdateAsync(Guid id, UpdateCountryDto dto)
    {
        var country = await _repository.GetByIdAsync(id);

        if (country == null)
            throw new Exception("Country not found.");

        country.Code = dto.Code;
        country.Name = dto.Name;
        country.CurrencyCode = dto.CurrencyCode;
        country.IsActive = dto.IsActive;

        await _repository.UpdateAsync(country);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }
}