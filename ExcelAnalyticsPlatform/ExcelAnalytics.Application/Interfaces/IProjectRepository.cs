using ExcelAnalytics.Domain.Entities;

namespace ExcelAnalytics.Application.Interfaces;

public interface IProjectRepository
{
    Task<List<Project>> GetAllAsync();

    Task<Project?> GetByIdAsync(Guid id);

    Task<bool> NameExistsAsync(string name, Guid? excludeId = null);

    Task<int> GetRateConfigurationCountAsync(Guid projectId);

    Task AddAsync(Project project);

    Task UpdateAsync(Project project);

    Task DeleteAsync(Guid id);
}
