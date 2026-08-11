using ExcelAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExcelAnalytics.Infrastructure.Persistence.Configurations;

public class RateConfigurationConfiguration : IEntityTypeConfiguration<RateConfiguration>
{
    public void Configure(EntityTypeBuilder<RateConfiguration> builder)
    {
        builder.ToTable("RateConfigurations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Rate)
               .HasColumnType("numeric(18,2)");

        builder.HasOne(x => x.Country)
               .WithMany()
               .HasForeignKey(x => x.CountryId);
    }
}