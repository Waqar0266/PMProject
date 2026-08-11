using ExcelAnalytics.Application.Interfaces;
using ExcelAnalytics.Domain.Entities;
using ExcelAnalytics.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ExcelAnalytics.Infrastructure.Repositories;

public class RateConfigurationRepository : IRateConfigurationRepository
{
    private readonly ApplicationDbContext _context;

    public RateConfigurationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<RateConfiguration>> GetAllAsync()
    {
        return await _context.RateConfigurations
            .Include(x => x.Country) .Include(x => x.Currency)
            .OrderByDescending(x => x.Year)
            .ThenByDescending(x => x.Month)
            .ToListAsync();
    }

    public async Task<RateConfiguration?> GetByIdAsync(Guid id)
    {
        return await _context.RateConfigurations
            .Include(x => x.Country) .Include(x => x.Currency)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(RateConfiguration rate)
    {
        await _context.RateConfigurations.AddAsync(rate);

        await _context.SaveChangesAsync();

        var country = await _context.Countries
            .FirstOrDefaultAsync(x => x.Id == rate.CountryId);

        if (country != null)
        {
            var all = await _context.AllocationRecords.ToListAsync();

            Console.WriteLine($"Country Code : {country.Code}");
            Console.WriteLine($"Year         : {rate.Year}");
            Console.WriteLine($"Month        : {rate.Month}");

            foreach (var a in all)
            {
                Console.WriteLine(
                    $"{a.EmployeeName} | {a.AllocatedFor} | {a.Month} | {a.Year}");
            }
            var allocations = await _context.AllocationRecords
               .Where(x =>
    x.AllocatedFor == country.Code &&
    x.Year == rate.Year &&
    x.Month == rate.Month)
                .ToListAsync();

            foreach (var allocation in allocations)
            {
                allocation.Rate = rate.Rate;
                allocation.Revenue =
                    allocation.FinalAllocationDays * rate.Rate;
            }

            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateAsync(RateConfiguration rate)
    {
        // Update Rate Configuration
        _context.RateConfigurations.Update(rate);
        await _context.SaveChangesAsync();

        // Load Country and Currency
        var updatedRate = await _context.RateConfigurations
            .Include(x => x.Country)
            .Include(x => x.Currency)
            .FirstOrDefaultAsync(x => x.Id == rate.Id);

        if (updatedRate == null)
            return;

        var allocations = await _context.AllocationRecords
            .Where(x =>
                x.AllocatedFor == updatedRate.Country.Code &&
                x.Year == updatedRate.Year &&
                x.Month == updatedRate.Month)
            .ToListAsync();

        foreach (var allocation in allocations)
        {
            allocation.Rate = updatedRate.Rate;

            allocation.Revenue =
                allocation.FinalAllocationDays * updatedRate.Rate;

            allocation.RevenueUSD =
                allocation.Revenue * updatedRate.Currency.ExchangeRateToUSD;
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var rate = await _context.RateConfigurations.FindAsync(id);

        if (rate == null)
            return;

        _context.RateConfigurations.Remove(rate);
        await _context.SaveChangesAsync();
    }
    public async Task<RateConfiguration?> GetByCountryYearMonthAsync(
    string countryCode,
    int year,
    int month)
    {
        return await _context.RateConfigurations
            .Include(x => x.Country) .Include(x => x.Currency)
            .FirstOrDefaultAsync(x =>
                x.Country.Code == countryCode &&
                x.Year == year &&
                x.Month == month);
    }
}