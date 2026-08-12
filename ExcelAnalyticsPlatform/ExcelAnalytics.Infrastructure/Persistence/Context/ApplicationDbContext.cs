using ExcelAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExcelAnalytics.Infrastructure.Persistence.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Country> Countries => Set<Country>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<RateConfiguration> RateConfigurations => Set<RateConfiguration>();
    public DbSet<AllocationRecord> AllocationRecords => Set<AllocationRecord>();
    public DbSet<Currency> Currencies => Set<Currency>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<RateConfiguration>()
    .HasOne(x => x.Currency)
    .WithMany()
    .HasForeignKey(x => x.CurrencyId)
    .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        modelBuilder.Entity<AllocationRecord>()
    .Property(x => x.StartDate)
    .HasColumnType("timestamp without time zone");

        modelBuilder.Entity<AllocationRecord>()
            .Property(x => x.FinishDate)
            .HasColumnType("timestamp without time zone");
    }
}