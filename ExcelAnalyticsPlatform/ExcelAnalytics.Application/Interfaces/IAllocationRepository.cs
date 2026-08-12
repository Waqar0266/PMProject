using ExcelAnalytics.Domain.Entities;

namespace ExcelAnalytics.Application.Interfaces;

public interface IAllocationRepository
{
    Task AddRangeAsync(List<AllocationRecord> records);

    Task SaveAsync();

    Task DeleteAllAsync();

    Task AddAsync(AllocationRecord record);

  

    Task<List<AllocationRecord>> GetAllAsync();
    Task DeleteByMonthYearAsync(int year, int month);
    Task UpdateProjectNameAsync(string oldName, string newName);
}