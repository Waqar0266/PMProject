using ExcelAnalytics.Application.DTOs.Project;
using ExcelAnalytics.Application.Interfaces;
using ExcelAnalytics.Domain.Entities;

namespace ExcelAnalytics.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _repository;
    private readonly IAllocationRepository _allocationRepository;

    public ProjectService(
        IProjectRepository repository,
        IAllocationRepository allocationRepository)
    {
        _repository = repository;
        _allocationRepository = allocationRepository;
    }

    public async Task<List<ProjectDto>> GetAllAsync()
    {
        var projects = await _repository.GetAllAsync();

        return projects.Select(x => new ProjectDto
        {
            Id = x.Id,
            Name = x.Name,
            IsActive = x.IsActive,
            RateConfigurationCount = x.RateConfigurations.Count
        }).ToList();
    }

    public async Task<ProjectDto?> GetByIdAsync(Guid id)
    {
        var project = await _repository.GetByIdAsync(id);

        if (project == null)
            return null;

        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            IsActive = project.IsActive,
            RateConfigurationCount = project.RateConfigurations.Count
        };
    }

    public async Task CreateAsync(CreateProjectDto dto)
    {
        var name = dto.Name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidOperationException("Project name is required.");

        if (await _repository.NameExistsAsync(name))
            throw new InvalidOperationException($"Project '{name}' already exists.");

        var project = new Project
        {
            Name = name,
            IsActive = true
        };

        await _repository.AddAsync(project);
    }

    public async Task UpdateAsync(Guid id, UpdateProjectDto dto)
    {
        var project = await _repository.GetByIdAsync(id);

        if (project == null)
            throw new InvalidOperationException("Project not found.");

        var name = dto.Name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidOperationException("Project name is required.");

        if (await _repository.NameExistsAsync(name, id))
            throw new InvalidOperationException($"Project '{name}' already exists.");

        var oldName = project.Name;

        project.Name = name;
        project.IsActive = dto.IsActive;

        await _repository.UpdateAsync(project);

        if (!oldName.Equals(name, StringComparison.OrdinalIgnoreCase))
            await _allocationRepository.UpdateProjectNameAsync(oldName, name);
    }

    public async Task DeleteAsync(Guid id)
    {
        var project = await _repository.GetByIdAsync(id);

        if (project == null)
            throw new InvalidOperationException("Project not found.");

        var rateCount = await _repository.GetRateConfigurationCountAsync(id);

        if (rateCount > 0)
            throw new InvalidOperationException(
                $"Cannot delete project '{project.Name}' because it is linked to {rateCount} rate configuration(s). Delete those rates first.");

        await _repository.DeleteAsync(id);
    }
}
