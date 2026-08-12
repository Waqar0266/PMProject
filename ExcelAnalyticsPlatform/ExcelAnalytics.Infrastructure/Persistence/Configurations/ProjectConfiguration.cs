using ExcelAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExcelAnalytics.Infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .HasMaxLength(200)
               .IsRequired();

        builder.HasIndex(x => x.Name)
               .IsUnique();

        builder.HasMany(x => x.RateConfigurations)
               .WithOne(x => x.Project)
               .HasForeignKey(x => x.ProjectId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
