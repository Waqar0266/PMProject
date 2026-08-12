using ExcelAnalytics.Application.Interfaces;
using ExcelAnalytics.Domain.Entities;
using ExcelAnalytics.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ExcelAnalytics.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Project>> GetAllAsync()
    {
        return await _context.Projects
            .Include(x => x.RateConfigurations)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<Project?> GetByIdAsync(Guid id)
    {
        return await _context.Projects
            .Include(x => x.RateConfigurations)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> NameExistsAsync(string name, Guid? excludeId = null)
    {
        var query = _context.Projects
            .Where(x => x.Name.ToLower() == name.Trim().ToLower());

        if (excludeId.HasValue)
            query = query.Where(x => x.Id != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task<int> GetRateConfigurationCountAsync(Guid projectId)
    {
        return await _context.RateConfigurations
            .CountAsync(x => x.ProjectId == projectId);
    }

    public async Task AddAsync(Project project)
    {
        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Project project)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var project = await _context.Projects.FindAsync(id);

        if (project == null)
            return;

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
    }
}
