namespace ExcelAnalytics.Application.DTOs.RateConfiguration;

public class RateConfigurationDto
{
    public Guid Id { get; set; }
    public Guid CountryId { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Rate { get; set; }
    public Guid CurrencyId { get; set; }

public string CurrencyName { get; set; } = "";

    public string CurrencyCode { get; set; } = "";

    public string CurrencySymbol { get; set; } = "";
}