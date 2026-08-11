using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelAnalytics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRevenueUSD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "RevenueUSD",
                table: "AllocationRecords",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RevenueUSD",
                table: "AllocationRecords");
        }
    }
}
