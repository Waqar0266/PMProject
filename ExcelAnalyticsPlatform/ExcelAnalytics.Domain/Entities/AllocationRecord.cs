using ExcelAnalytics.Domain.Common;
namespace ExcelAnalytics.Domain.Entities;
public class AllocationRecord : BaseEntity
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string DepartmentType { get; set; } = string.Empty;
    public string Team { get; set; } = string.Empty;
    public string Groups { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string Stream { get; set; } = string.Empty;
    public string Product { get; set; } = string.Empty;
    public string SubTeam { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? FinishDate { get; set; }
    public decimal PercentAllocated { get; set; }
    public decimal PercentEffective { get; set; }
    public string ReportingTL { get; set; } = string.Empty;
    public string AllocatedFor { get; set; } = string.Empty;
    public string OfficeLocation { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal AllocationDays { get; set; }
    public decimal ResourceCount { get; set; }
    public decimal Leaves { get; set; }
    public decimal FinalAllocationDays { get; set; }

    // Calculated later
    public decimal Rate { get; set; }

    // Revenue in the selected currency
    public decimal Revenue { get; set; }

    // Revenue converted to USD
    public decimal RevenueUSD { get; set; }
    public int Year { get; set; }

    public int Month { get; set; }
}