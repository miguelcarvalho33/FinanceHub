using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceHub.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddSavingsBalanceToBankStatement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SavingsBalance",
                table: "BankStatements",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SavingsBalance",
                table: "BankStatements");
        }
    }
}
