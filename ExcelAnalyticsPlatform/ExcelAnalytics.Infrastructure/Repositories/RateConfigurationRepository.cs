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
            .Include(x => x.Country)
            .Include(x => x.Project)
            .Include(x => x.Currency)
            .OrderByDescending(x => x.Year)
            .ThenByDescending(x => x.Month)
            .ToListAsync();
    }

    public async Task<RateConfiguration?> GetByIdAsync(Guid id)
    {
        return await _context.RateConfigurations
            .Include(x => x.Country)
            .Include(x => x.Project)
            .Include(x => x.Currency)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(RateConfiguration rate)
    {
        await _context.RateConfigurations.AddAsync(rate);
        await _context.SaveChangesAsync();

        await RecalculateAllocationsAsync(rate.Id);
    }

    public async Task UpdateAsync(RateConfiguration rate)
    {
        _context.RateConfigurations.Update(rate);
        await _context.SaveChangesAsync();

        await RecalculateAllocationsAsync(rate.Id);
    }

    public async Task DeleteAsync(Guid id)
    {
        var rate = await _context.RateConfigurations.FindAsync(id);

        if (rate == null)
            return;

        _context.RateConfigurations.Remove(rate);
        await _context.SaveChangesAsync();
    }

    public async Task<RateConfiguration?> GetByCountryProjectYearMonthAsync(
        string countryCode,
        string projectName,
        int year,
        int month)
    {
        return await _context.RateConfigurations
            .Include(x => x.Country)
            .Include(x => x.Project)
            .Include(x => x.Currency)
            .FirstOrDefaultAsync(x =>
                x.Country.Code == countryCode &&
                x.Project.Name.ToLower() == projectName.Trim().ToLower() &&
                x.Year == year &&
                x.Month == month &&
                x.IsActive);
    }

    private async Task RecalculateAllocationsAsync(Guid rateId)
    {
        var rateConfiguration = await _context.RateConfigurations
            .Include(x => x.Country)
            .Include(x => x.Project)
            .Include(x => x.Currency)
            .FirstOrDefaultAsync(x => x.Id == rateId);

        if (rateConfiguration == null)
            return;

        var allocations = await _context.AllocationRecords
            .Where(x =>
                x.AllocatedFor == rateConfiguration.Country.Code &&
                x.ProjectName.ToLower() == rateConfiguration.Project.Name.ToLower() &&
                x.Year == rateConfiguration.Year &&
                x.Month == rateConfiguration.Month)
            .ToListAsync();

        foreach (var allocation in allocations)
        {
            allocation.Rate = rateConfiguration.Rate;
            allocation.Revenue = allocation.FinalAllocationDays * rateConfiguration.Rate;
            allocation.RevenueUSD =
                allocation.Revenue * rateConfiguration.Currency.ExchangeRateToUSD;
        }

        await _context.SaveChangesAsync();
    }
}
