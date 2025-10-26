using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceHub.Core.Models
{
 [Table("BankStatements")]
 public class BankStatement
 {
 [Key] public int Id { get; set; }
 [Required, MaxLength(50)] public string Bank { get; set; }
 [MaxLength(50)] public string ClientNumber { get; set; }
 [MaxLength(50)] public string StatementNumber { get; set; }
 public DateTime? EmissionDate { get; set; }
 [MaxLength(100)] public string Period { get; set; }
 [MaxLength(34)] public string IBAN { get; set; }
 [MaxLength(25)] public string NIB { get; set; }
 [MaxLength(20)] public string SWIFT { get; set; }
 [Column(TypeName="decimal(18,2)")] public decimal? PreviousBalance { get; set; }
 [Column(TypeName="decimal(18,2)")] public decimal? FinalBalance { get; set; }

 public ICollection<Loan> Loans { get; set; } = new List<Loan>();
 public ICollection<DirectDebit> DirectDebits { get; set; } = new List<DirectDebit>();
 public ICollection<Card> Cards { get; set; } = new List<Card>();
 public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
 }

 [Table("Loans")]
 public class Loan
 {
 [Key] public int Id { get; set; }
 public int BankStatementId { get; set; }
 public BankStatement BankStatement { get; set; }

 [MaxLength(200)] public string LoanType { get; set; }
 [MaxLength(50)] public string AccountNumber { get; set; }
 [Column(TypeName="decimal(18,2)")] public decimal? ContractedValue { get; set; }
 public DateTime? ContractDate { get; set; }
 [Column(TypeName="decimal(5,3)")] public decimal? IndexRate { get; set; }
 [Column(TypeName="decimal(5,3)")] public decimal? Spread { get; set; }
 [Column(TypeName="decimal(18,2)")] public decimal? TANL { get; set; }
 [Column(TypeName="decimal(18,2)")] public decimal? OutstandingBalance { get; set; }

 [Column(TypeName="decimal(18,2)")] public decimal? NextCapitalPayment { get; set; }
 [Column(TypeName="decimal(18,2)")] public decimal? NextInterestPayment { get; set; }
 public DateTime? NextPaymentDate { get; set; }

 public ICollection<LoanPayment> Payments { get; set; } = new List<LoanPayment>();
 }

 [Table("LoanPayments")]
 public class LoanPayment
 {
 [Key] public int Id { get; set; }
 public int LoanId { get; set; }
 public Loan Loan { get; set; }

 public int? InstallmentNumber { get; set; }
 public DateTime MovementDate { get; set; }
 public DateTime? ValueDate { get; set; }

 [Column(TypeName="decimal(18,2)")] public decimal? CapitalAmount { get; set; }
 [Column(TypeName="decimal(18,2)")] public decimal? InterestAmount { get; set; }
 [NotMapped] public decimal TotalAmount => (CapitalAmount ??0) + (InterestAmount ??0);

 [MaxLength(200)] public string Description { get; set; }
 [MaxLength(100)] public string SourceLine { get; set; }
 }

 [Table("DirectDebits")]
 public class DirectDebit
 {
 [Key] public int Id { get; set; }
 public int BankStatementId { get; set; }
 public BankStatement BankStatement { get; set; }

 public DateTime? CreationDate { get; set; }
 [MaxLength(200)] public string CreditorName { get; set; }
 [MaxLength(50)] public string EntityNumber { get; set; }
 [MaxLength(50)] public string AuthorizationNumber { get; set; }
 [MaxLength(50)] public string LimitValueRaw { get; set; }
 }

 [Table("Cards")]
 public class Card
 {
 [Key] public int Id { get; set; }
 public int BankStatementId { get; set; }
 public BankStatement BankStatement { get; set; }

 [MaxLength(100)] public string CardName { get; set; }
 [MaxLength(30)] public string MaskedNumber { get; set; }
 [MaxLength(200)] public string Holder { get; set; }
 [MaxLength(10)] public string Currency { get; set; }
 [Column(TypeName="decimal(18,2)")] public decimal? CreditUsed { get; set; }
 }
}
