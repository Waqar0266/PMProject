using ClosedXML.Excel;
using ExcelAnalytics.Application.Interfaces;
using ExcelAnalytics.Domain.Entities;
using ExcelAnalytics.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ExcelAnalytics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly IAllocationRepository _allocationRepository;
    private readonly IRateConfigurationRepository _rateConfigurationRepository;

    public UploadController(
        IAllocationRepository allocationRepository,
        IRateConfigurationRepository rateConfigurationRepository)
    {
        _allocationRepository = allocationRepository;
        _rateConfigurationRepository = rateConfigurationRepository;
    }
    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest("Please select an Excel file.");

            // Save uploaded file
            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(
                uploadsFolder,
                file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Open workbook
            using var workbook = new XLWorkbook(filePath);

            var worksheet = workbook.Worksheet(1);

            var rows = worksheet.RowsUsed().ToList();

            if (!rows.Any())
                return BadRequest("Worksheet is empty.");

            // Find Header Row
            IXLRow? headerRow = null;

            foreach (var row in rows)
            {
                if (row.CellsUsed().Any(c =>
                    c.GetString().Trim()
                        .Equals("Emp Code",
                            StringComparison.OrdinalIgnoreCase)))
                {
                    headerRow = row;
                    break;
                }
            }

            if (headerRow == null)
                return BadRequest("Header row not found.");

            // Build Header Dictionary
            Dictionary<string, int> headerMap = new();

            foreach (var cell in headerRow.CellsUsed())
            {
                var header = cell.GetString().Trim();

                if (!headerMap.ContainsKey(header))
                    headerMap.Add(header, cell.Address.ColumnNumber);
            }

            // Extract Month & Year
            var period = GetAllocationPeriod(headerMap);

            int allocationYear = period.Year;
            int allocationMonth = period.Month;

            // Delete only same month's data
            await _allocationRepository.DeleteByMonthYearAsync(
                allocationYear,
                allocationMonth);

            List<AllocationRecord> allocations = new();
            List<(string EmployeeCode, string EmployeeName, string Country, string Project)> missingRateRows = new();

            foreach (var row in worksheet.RowsUsed()
                .Where(x => x.RowNumber() > headerRow.RowNumber()))
            {
                if (string.IsNullOrWhiteSpace(
                    GetCell(row, headerMap, "Emp Code")))
                    continue;

                decimal allocationDays =
                    ToDecimal(GetStartsWith(row, headerMap, "Allocation Days"));

                decimal resourceCount =
                    ToDecimal(GetStartsWith(row, headerMap, "Resource Count"));

                decimal leaves =
                    ToDecimal(GetStartsWith(row, headerMap, "Allocated Leaves"));

                decimal finalDays =
                    ToDecimal(GetStartsWith(row, headerMap,
                        "Allocation Days Minus"));

                decimal percentAllocated =
                    ToDecimal(GetCell(row, headerMap,
                        "Percent Allocated"));

                decimal percentEffective =
                    ToDecimal(GetCell(row, headerMap,
                        "Percent Effective"));

                string countryCode =
                    GetCell(row, headerMap, "Allocated For");

                string projectName =
                    GetCell(row, headerMap, "Project Name");

                var rateConfiguration =
                    await _rateConfigurationRepository.GetByCountryProjectYearMonthAsync(
                        countryCode,
                        projectName,
                        allocationYear,
                        allocationMonth);

                decimal rate = rateConfiguration?.Rate ?? 0;

                if (rateConfiguration == null)
                {
                    missingRateRows.Add((
                        GetCell(row, headerMap, "Emp Code"),
                        GetCell(row, headerMap, "Resource name"),
                        countryCode,
                        projectName));
                }

                // Revenue in configured currency
                decimal revenue = finalDays * rate;

                // Convert to USD
                decimal exchangeRate =
                    rateConfiguration?.Currency?.ExchangeRateToUSD ?? 1;

                decimal revenueUSD = revenue * exchangeRate;
                AllocationRecord allocation = new AllocationRecord
                {
                    EmployeeCode = GetCell(row, headerMap, "Emp Code"),
                    EmployeeName = GetCell(row, headerMap, "Resource name"),
                    DepartmentType = GetCell(row, headerMap, "Department Type"),
                    Team = GetCell(row, headerMap, "Team"),
                    Groups = GetCell(row, headerMap, "Groups"),
                    ProjectName = GetCell(row, headerMap, "Project Name"),
                    Stream = GetCell(row, headerMap, "Stream"),
                    Product = GetCell(row, headerMap, "Product"),
                    SubTeam = GetCell(row, headerMap, "Sub Team"),
                    Designation = GetCell(row, headerMap, "Designation"),

                    StartDate = null,
                    FinishDate = null,

                    PercentAllocated = percentAllocated,
                    PercentEffective = percentEffective,
                    ReportingTL = GetCell(row, headerMap, "Reporting TL"),
                    AllocatedFor = countryCode,
                    OfficeLocation = GetCell(row, headerMap, "Office Location"),
                    Grade = GetCell(row, headerMap, "Grade"),
                    Email = GetCell(row, headerMap, "Email"),

                    AllocationDays = allocationDays,
                    ResourceCount = resourceCount,
                    Leaves = leaves,
                    FinalAllocationDays = finalDays,

                    Year = allocationYear,
                    Month = allocationMonth,

                    Rate = rate,
                    Revenue = revenue,
                    RevenueUSD = revenueUSD
                };

                allocations.Add(allocation);
            }

            await _allocationRepository.AddRangeAsync(allocations);
            await _allocationRepository.SaveAsync();

            var missingRateWarnings = missingRateRows
                .GroupBy(x => new { x.Country, x.Project })
                .Select(g => new
                {
                    Country = g.Key.Country,
                    Project = g.Key.Project,
                    Year = allocationYear,
                    Month = allocationMonth,
                    MonthName = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(allocationMonth),
                    RowCount = g.Count(),
                    Employees = g.Select(x => $"{x.EmployeeName} ({x.EmployeeCode})").Take(10).ToList()
                })
                .ToList();

            return Ok(new
            {
                Message = missingRateWarnings.Any()
                    ? "Excel imported with warnings — some rows have no matching rate."
                    : "Excel imported successfully.",
                RecordsImported = allocations.Count,
                RecordsWithMissingRate = missingRateRows.Count,
                MissingRateWarnings = missingRateWarnings
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    //[HttpPost]
    //public async Task<IActionResult> Upload(IFormFile file)
    //{
    //    try
    //    {
    //        if (file == null || file.Length == 0)
    //            return BadRequest("Please select an Excel file.");

                //        // Delete previous uploaded data
                //        await _allocationRepository.DeleteAllAsync();

                //        // Create Upload folder
                //        var uploadsFolder = Path.Combine(
                //            Directory.GetCurrentDirectory(),
                //            "Uploads");

                //        if (!Directory.Exists(uploadsFolder))
                //            Directory.CreateDirectory(uploadsFolder);

                //        var filePath = Path.Combine(uploadsFolder, file.FileName);

                //        using (var stream = new FileStream(filePath, FileMode.Create))
                //        {
                //            await file.CopyToAsync(stream);
                //        }

                //        using var workbook = new XLWorkbook(filePath);

                //        var worksheet = workbook.Worksheet(1);

                //        var rows = worksheet.RowsUsed().ToList();

                //        if (!rows.Any())
                //            return BadRequest("Worksheet is empty.");

                //        // Find Header Row
                //        IXLRow? headerRow = null;

                //        foreach (var row in rows)
                //        {
                //            if (row.CellsUsed().Any(x =>
                //                x.GetString().Trim()
                //                .Equals("Emp Code",
                //                    StringComparison.OrdinalIgnoreCase)))
                //            {
                //                headerRow = row;
                //                break;
                //            }
                //        }

                //        if (headerRow == null)
                //            return BadRequest("Header row not found.");

                //        // Build Header Dictionary
                //        Dictionary<string, int> headerMap = new();

                //        foreach (var cell in headerRow.CellsUsed())
                //        {
                //            var name = cell.GetString().Trim();

                //            if (!headerMap.ContainsKey(name))
                //                headerMap.Add(name, cell.Address.ColumnNumber);
                //        }

                //        List<AllocationRecord> allocations = new();

                //        foreach (var row in worksheet.RowsUsed()
                //                     .Where(x => x.RowNumber() > headerRow.RowNumber()))
                //        {
                //            if (string.IsNullOrWhiteSpace(
                //                    GetCell(row, headerMap, "Emp Code")))
                //                continue;

                //            decimal allocationDays =
                //                ToDecimal(GetStartsWith(row, headerMap, "Allocation Days"));

                //            decimal resourceCount =
                //                ToDecimal(GetStartsWith(row, headerMap, "Resource Count"));

                //            decimal leaves =
                //                ToDecimal(GetStartsWith(row, headerMap, "Allocated Leaves"));

                //            decimal finalDays =
                //                ToDecimal(GetStartsWith(row, headerMap,
                //                    "Allocation Days Minus"));

                //            DateTime? startDate =
                //                ToDate(GetCell(row, headerMap, "Start Date"));

                //            DateTime? finishDate =
                //                ToDate(GetCell(row, headerMap, "Finish Date"));

                //            decimal percentAllocated =
                //                ToDecimal(GetCell(row, headerMap, "Percent Allocated"));

                //            decimal percentEffective =
                //                ToDecimal(GetCell(row, headerMap, "Percent Effective"));

                //            AllocationRecord allocation = new AllocationRecord
                //            {
                //                EmployeeCode = GetCell(row, headerMap, "Emp Code"),
                //                EmployeeName = GetCell(row, headerMap, "Resource name"),
                //                DepartmentType = GetCell(row, headerMap, "Department Type"),
                //                Team = GetCell(row, headerMap, "Team"),
                //                Groups = GetCell(row, headerMap, "Groups"),
                //                ProjectName = GetCell(row, headerMap, "Project Name"),
                //                Stream = GetCell(row, headerMap, "Stream"),
                //                Product = GetCell(row, headerMap, "Product"),
                //                SubTeam = GetCell(row, headerMap, "Sub Team"),
                //                Designation = GetCell(row, headerMap, "Designation"),
                //                StartDate = startDate,
                //                FinishDate = finishDate,
                //                PercentAllocated = percentAllocated,
                //                PercentEffective = percentEffective,
                //                ReportingTL = GetCell(row, headerMap, "Reporting TL"),
                //                AllocatedFor = GetCell(row, headerMap, "Allocated For"),
                //                OfficeLocation = GetCell(row, headerMap, "Office Location"),
                //                Grade = GetCell(row, headerMap, "Grade"),
                //                Email = GetCell(row, headerMap, "Email"),
                //                AllocationDays = allocationDays,
                //                ResourceCount = resourceCount,
                //                Leaves = leaves,
                //                FinalAllocationDays = finalDays,

                //                // Next step we'll calculate these
                //                Rate = 0,
                //                Revenue = 0
                //            };

                //            allocations.Add(allocation);
                //        }

                //        await _allocationRepository.AddRangeAsync(allocations);
                //        await _allocationRepository.SaveAsync();

                //        return Ok(new
                //        {
                //            Message = "Excel imported successfully.",
                //            RecordsImported = allocations.Count
                //        });
                //    }
                //    catch (Exception ex)
                //    {
                //        return BadRequest(ex.Message);
                //    }
                //}






    private string GetCell(
      IXLRow row,
      Dictionary<string, int> map,
      string column)
    {
        if (!map.ContainsKey(column))
            return "";

        return row.Cell(map[column]).GetString().Trim();
    }

    private string GetStartsWith(
        IXLRow row,
        Dictionary<string, int> map,
        string startsWith)
    {
        var key = map.Keys.FirstOrDefault(x =>
            x.StartsWith(startsWith,
                StringComparison.OrdinalIgnoreCase));

        if (key == null)
            return "";

        return row.Cell(map[key]).GetString().Trim();
    }

    private decimal ToDecimal(string value)
    {
        decimal.TryParse(value, out decimal result);
        return result;
    }

    private DateTime? ToDate(string value)
    {
        if (DateTime.TryParse(value, out DateTime result))
            return result;

        return null;
    }
    private (int Year, int Month) GetAllocationPeriod(
    Dictionary<string, int> headerMap)
    {
        var header = headerMap.Keys.FirstOrDefault(x =>
            x.StartsWith("Allocation Days",
                StringComparison.OrdinalIgnoreCase));

        if (string.IsNullOrEmpty(header))
            throw new Exception("Allocation Days column not found.");

        // Example:
        // Allocation Days 01/Jun/2026-30/Jun/2026

        var match = Regex.Match(
    header,
    @"\d{2}/(?<month>[A-Za-z]+)/(?<year>\d{4})",
    RegexOptions.IgnoreCase);

        if (!match.Success)
            throw new Exception("Unable to determine Month/Year.");

        string monthName = match.Groups["month"].Value;
        int month = GetMonthNumber(monthName);

        int year = int.Parse(match.Groups["year"].Value);

        return (year, month);
    }
    private int GetMonthNumber(string monthName)
    {
        monthName = monthName.Trim();

        // Try abbreviated month (Jan, Feb, Jul)
        if (DateTime.TryParseExact(
                monthName,
                "MMM",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var date))
        {
            return date.Month;
        }

        // Try full month (January, February, July)
        if (DateTime.TryParseExact(
                monthName,
                "MMMM",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out date))
        {
            return date.Month;
        }

        throw new Exception($"Invalid month name: {monthName}");
    }
}
