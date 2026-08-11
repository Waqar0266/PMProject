namespace ExcelAnalytics.Application.DTOs.Currency;

public class UpdateCurrencyDto
{
    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Symbol { get; set; } = string.Empty;

    public decimal ExchangeRateToUSD { get; set; }

    public bool IsActive { get; set; }
}