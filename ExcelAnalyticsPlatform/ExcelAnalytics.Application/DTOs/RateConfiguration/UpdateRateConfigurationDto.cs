public class UpdateRateConfigurationDto
{
    public Guid CountryId { get; set; }

    public Guid ProjectId { get; set; }

    public Guid CurrencyId { get; set; }

    public int Year { get; set; }

    public int Month { get; set; }

    public decimal Rate { get; set; }

    public bool IsActive { get; set; }
}