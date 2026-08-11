using ExcelAnalytics.Domain.Common;

namespace ExcelAnalytics.Domain.Entities;

public class Country : BaseEntity
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string CurrencyCode { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}