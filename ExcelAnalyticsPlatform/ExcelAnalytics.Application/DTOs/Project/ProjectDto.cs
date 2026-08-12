namespace ExcelAnalytics.Application.DTOs.Project;

public class ProjectDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public int RateConfigurationCount { get; set; }
}
