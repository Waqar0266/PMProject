using ExcelAnalytics.Domain.Common;

namespace ExcelAnalytics.Domain.Entities;

public class RateConfiguration : BaseEntity
{
    public Guid CountryId { get; set; }

    public Country Country { get; set; } = null!;

    public int Year { get; set; }

    public int Month { get; set; }

    public decimal Rate { get; set; }

    public bool IsActive { get; set; } = true;
    public Guid CurrencyId { get; set; }

    public Currency Currency { get; set; } = null!;
}