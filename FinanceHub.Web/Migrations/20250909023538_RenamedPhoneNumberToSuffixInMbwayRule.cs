using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceHub.Web.Migrations
{
    /// <inheritdoc />
    public partial class RenamedPhoneNumberToSuffixInMbwayRule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "MbwayRules",
                newName: "PhoneNumberSuffix");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhoneNumberSuffix",
                table: "MbwayRules",
                newName: "PhoneNumber");
        }
    }
}
