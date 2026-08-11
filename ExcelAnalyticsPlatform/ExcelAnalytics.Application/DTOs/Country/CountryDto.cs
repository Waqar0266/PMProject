namespace ExcelAnalytics.Application.DTOs.Country;

public class CountryDto
{
    public Guid Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string CurrencyCode { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}