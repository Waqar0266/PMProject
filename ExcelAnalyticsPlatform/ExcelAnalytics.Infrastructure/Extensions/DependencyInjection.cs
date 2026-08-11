using ExcelAnalytics.Infrastructure.Persistence.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using ExcelAnalytics.Application.Interfaces;
using ExcelAnalytics.Application.Services;
using ExcelAnalytics.Infrastructure.Repositories;
using ExcelAnalytics.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ExcelAnalytics.Application.Services;
using ExcelAnalytics.Infrastructure.Repositories;
using ExcelAnalytics.Application.Interfaces;
using ExcelAnalytics.Application.Services;
using ExcelAnalytics.Infrastructure.Repositories;
namespace ExcelAnalytics.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<IRateConfigurationRepository, RateConfigurationRepository>();
        services.AddScoped<IRateConfigurationService, RateConfigurationService>();
        services.AddScoped<IAllocationRepository, AllocationRepository>();
        services.AddScoped<ICurrencyRepository, CurrencyRepository>();
        services.AddScoped<ICurrencyService, CurrencyService>();
        return services;
    }
}