using ExcelAnalytics.Application.DTOs.Currency;
using ExcelAnalytics.Application.Interfaces;
using ExcelAnalytics.Domain.Entities;

namespace ExcelAnalytics.Application.Services;

public class CurrencyService : ICurrencyService
{
    private readonly ICurrencyRepository _repository;

    public CurrencyService(ICurrencyRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CurrencyDto>> GetAllAsync()
    {
        var currencies = await _repository.GetAllAsync();

        return currencies.Select(x => new CurrencyDto
        {
            Id = x.Id,
            Name = x.Name,
            Code = x.Code,
            Symbol = x.Symbol,
            ExchangeRateToUSD = x.ExchangeRateToUSD,
            IsActive = x.IsActive
        }).ToList();
    }

    public async Task<CurrencyDto?> GetByIdAsync(Guid id)
    {
        var currency = await _repository.GetByIdAsync(id);

        if (currency == null)
            return null;

        return new CurrencyDto
        {
            Id = currency.Id,
            Name = currency.Name,
            Code = currency.Code,
            Symbol = currency.Symbol,
            ExchangeRateToUSD = currency.ExchangeRateToUSD,
            IsActive = currency.IsActive
        };
    }

    public async Task CreateAsync(CreateCurrencyDto dto)
    {
        var currency = new Currency
        {
            Name = dto.Name,
            Code = dto.Code,
            Symbol = dto.Symbol,
            ExchangeRateToUSD = dto.ExchangeRateToUSD
        };

        await _repository.AddAsync(currency);
    }

    public async Task UpdateAsync(Guid id, UpdateCurrencyDto dto)
    {
        var currency = await _repository.GetByIdAsync(id);

        if (currency == null)
            throw new Exception("Currency not found.");

        currency.Name = dto.Name;
        currency.Code = dto.Code;
        currency.Symbol = dto.Symbol;
        currency.ExchangeRateToUSD = dto.ExchangeRateToUSD;
        currency.IsActive = dto.IsActive;

        await _repository.UpdateAsync(currency);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }
}