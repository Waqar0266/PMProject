using ExcelAnalytics.Application.Interfaces;
using ExcelAnalytics.Domain.Entities;
using ExcelAnalytics.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ExcelAnalytics.Infrastructure.Repositories;

public class CurrencyRepository : ICurrencyRepository
{
    private readonly ApplicationDbContext _context;

    public CurrencyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Currency>> GetAllAsync()
    {
        return await _context.Currencies
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<Currency?> GetByIdAsync(Guid id)
    {
        return await _context.Currencies
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(Currency currency)
    {
        await _context.Currencies.AddAsync(currency);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Currency currency)
    {
        _context.Currencies.Update(currency);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var currency = await _context.Currencies.FindAsync(id);

        if (currency == null)
            return;

        _context.Currencies.Remove(currency);
        await _context.SaveChangesAsync();
    }
}