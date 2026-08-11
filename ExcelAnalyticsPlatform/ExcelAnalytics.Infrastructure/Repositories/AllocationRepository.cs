using ExcelAnalytics.Application.Interfaces;
using ExcelAnalytics.Domain.Entities;
using ExcelAnalytics.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ExcelAnalytics.Infrastructure.Repositories;

public class AllocationRepository : IAllocationRepository
{
    private readonly ApplicationDbContext _context;

    public AllocationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AllocationRecord record)
    {
        await _context.AllocationRecords.AddAsync(record);
    }
    public async Task DeleteByMonthYearAsync(int year, int month)
    {
        var records = await _context.AllocationRecords
            .Where(x => x.Year == year && x.Month == month)
            .ToListAsync();

        _context.AllocationRecords.RemoveRange(records);

        await _context.SaveChangesAsync();
    }
    public async Task AddRangeAsync(List<AllocationRecord> records)
    {
        await _context.AllocationRecords.AddRangeAsync(records);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAllAsync()
    {
        _context.AllocationRecords.RemoveRange(_context.AllocationRecords);
        await _context.SaveChangesAsync();
    }

    public async Task<List<AllocationRecord>> GetAllAsync()
    {
        return await _context.AllocationRecords.ToListAsync();
    }
}