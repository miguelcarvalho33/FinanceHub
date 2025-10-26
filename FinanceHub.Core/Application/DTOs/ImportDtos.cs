using FinanceHub.Core.Models;

namespace FinanceHub.Core.Application.DTOs
{
 /// <summary>
 /// DTO for importing transactions from PDFs.
 /// </summary>
 public class TransactionDto
 {
 public DateTime MovementDate { get; set; }
 public DateTime ValueDate { get; set; }
 public string OriginalDescription { get; set; } = string.Empty;
 public decimal Amount { get; set; }
 public string Bank { get; set; } = string.Empty;
 }

 /// <summary>
 /// DTO for importing a bank statement.
 /// </summary>
 public class BankStatementDto
 {
 public string Bank { get; set; } = string.Empty;
 public string? ClientNumber { get; set; }
 public string? StatementNumber { get; set; }
 public DateTime? EmissionDate { get; set; }
 public string? Period { get; set; }
 public string? IBAN { get; set; }
 public string? NIB { get; set; }
 public string? SWIFT { get; set; }
 public decimal? PreviousBalance { get; set; }
 public decimal? FinalBalance { get; set; }
 public decimal? SavingsBalance { get; set; }
 }
}
