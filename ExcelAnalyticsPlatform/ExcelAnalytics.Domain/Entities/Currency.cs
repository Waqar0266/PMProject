using ExcelAnalytics.Domain.Common;

namespace ExcelAnalytics.Domain.Entities;

public class Currency : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Symbol { get; set; } = string.Empty;

    // Example:
    // USD = 1
    // CNY = 7.20
    // PKR = 285
    public decimal ExchangeRateToUSD { get; set; }

    public bool IsActive { get; set; } = true;
}