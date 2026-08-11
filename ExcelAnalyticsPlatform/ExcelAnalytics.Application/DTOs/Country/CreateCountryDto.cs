namespace ExcelAnalytics.Application.DTOs.Country;

public class CreateCountryDto
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string CurrencyCode { get; set; } = string.Empty;
}