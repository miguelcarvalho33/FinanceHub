using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceHub.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddBankStatementsLoansDirectDebitsCards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BankStatementId",
                table: "Transactions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BankStatements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Bank = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ClientNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    StatementNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    EmissionDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Period = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IBAN = table.Column<string>(type: "TEXT", maxLength: 34, nullable: false),
                    NIB = table.Column<string>(type: "TEXT", maxLength: 25, nullable: false),
                    SWIFT = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    PreviousBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FinalBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankStatements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BankStatementId = table.Column<int>(type: "INTEGER", nullable: false),
                    CardName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    MaskedNumber = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Holder = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    CreditUsed = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cards_BankStatements_BankStatementId",
                        column: x => x.BankStatementId,
                        principalTable: "BankStatements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DirectDebits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BankStatementId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreditorName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    EntityNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    AuthorizationNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LimitValueRaw = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectDebits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DirectDebits_BankStatements_BankStatementId",
                        column: x => x.BankStatementId,
                        principalTable: "BankStatements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Loans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BankStatementId = table.Column<int>(type: "INTEGER", nullable: false),
                    LoanType = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    AccountNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ContractedValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ContractDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IndexRate = table.Column<decimal>(type: "decimal(5,3)", nullable: true),
                    Spread = table.Column<decimal>(type: "decimal(5,3)", nullable: true),
                    TANL = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OutstandingBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NextCapitalPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NextInterestPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NextPaymentDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Loans_BankStatements_BankStatementId",
                        column: x => x.BankStatementId,
                        principalTable: "BankStatements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LoanId = table.Column<int>(type: "INTEGER", nullable: false),
                    InstallmentNumber = table.Column<int>(type: "INTEGER", nullable: true),
                    MovementDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ValueDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CapitalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InterestAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    SourceLine = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanPayments_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BankStatementId",
                table: "Transactions",
                column: "BankStatementId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_BankStatementId",
                table: "Cards",
                column: "BankStatementId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectDebits_BankStatementId",
                table: "DirectDebits",
                column: "BankStatementId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanPayments_LoanId",
                table: "LoanPayments",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_BankStatementId",
                table: "Loans",
                column: "BankStatementId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_BankStatements_BankStatementId",
                table: "Transactions",
                column: "BankStatementId",
                principalTable: "BankStatements",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_BankStatements_BankStatementId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "DirectDebits");

            migrationBuilder.DropTable(
                name: "LoanPayments");

            migrationBuilder.DropTable(
                name: "Loans");

            migrationBuilder.DropTable(
                name: "BankStatements");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_BankStatementId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "BankStatementId",
                table: "Transactions");
        }
    }
}
