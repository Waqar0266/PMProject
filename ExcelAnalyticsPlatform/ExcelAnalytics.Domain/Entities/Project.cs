using ExcelAnalytics.Domain.Common;

namespace ExcelAnalytics.Domain.Entities;

public class Project : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<RateConfiguration> RateConfigurations { get; set; } = new List<RateConfiguration>();
}
