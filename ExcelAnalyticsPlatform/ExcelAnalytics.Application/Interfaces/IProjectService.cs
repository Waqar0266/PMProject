using ExcelAnalytics.Application.DTOs.Project;

namespace ExcelAnalytics.Application.Interfaces;

public interface IProjectService
{
    Task<List<ProjectDto>> GetAllAsync();

    Task<ProjectDto?> GetByIdAsync(Guid id);

    Task CreateAsync(CreateProjectDto dto);

    Task UpdateAsync(Guid id, UpdateProjectDto dto);

    Task DeleteAsync(Guid id);
}
