using System.Globalization;
using System.Text.RegularExpressions;
using FinanceHub.Core.Models;
using FinanceHub.Core.Parsing;

namespace FinanceHub.Web.Parsers
{
 public class CgdExtratoGlobalParser : IPdfParser
 {
 public string BankName => "Caixa Geral de Depósitos";

 public bool CanParse(string text)
 {
 if (string.IsNullOrWhiteSpace(text)) return false;
 return text.Contains("Extrato global", StringComparison.OrdinalIgnoreCase)
 || text.Contains("Extrato Global", StringComparison.OrdinalIgnoreCase)
 || text.Contains("Caixa Geral de Depósitos", StringComparison.OrdinalIgnoreCase)
 || text.Contains("Crédito à habitação", StringComparison.OrdinalIgnoreCase);
 }

 public List<Transaction> Parse(string text)
 {
 var list = new List<Transaction>();
 if (string.IsNullOrWhiteSpace(text)) return list;

 var movementRegex = new Regex(
 "^(?<mov>\\d{4}-\\d{2}-\\d{2})\\s*(?<val>\\d{4}-\\d{2}-\\d{2})\\s+(?<desc>.*?)\\s+(?<amount>-?[\\d\\.\\,]+)\\s+(?<balance>-?[\\d\\.\\,]+)$",
 RegexOptions.Multiline | RegexOptions.CultureInvariant);

 foreach (Match m in movementRegex.Matches(text))
 {
 var mov = DateTime.Parse(m.Groups["mov"].Value);
 var val = DateTime.Parse(m.Groups["val"].Value);
 var desc = m.Groups["desc"].Value.Trim();
 var amount = ParsePtDecimal(m.Groups["amount"].Value) ??0m;

 list.Add(new Transaction
 {
 MovementDate = mov,
 ValueDate = val,
 OriginalDescription = desc,
 Amount = amount,
 Bank = BankName
 });
 }
 return list;
 }

 // Metadata parse helpers
 public BankStatement? ParseMetadata(string text)
 {
 if (string.IsNullOrWhiteSpace(text)) return null;
 var s = new BankStatement
 {
 Bank = BankName
 };

 s.ClientNumber = Regex.Match(text, @"Cliente\s*(?<v>\d+)", RegexOptions.IgnoreCase).Groups["v"].Value;
 s.StatementNumber = Regex.Match(text, @"Extrato\s*(n\.?º)?\s*(?<v>[\d\/]+)", RegexOptions.IgnoreCase).Groups["v"].Value;
 var em = Regex.Match(text, @"Emiss[aã]o\s*(?<d>\d{4}-\d{2}-\d{2})", RegexOptions.IgnoreCase);
 if (em.Success && DateTime.TryParse(em.Groups["d"].Value, out var ed)) s.EmissionDate = ed;
 var per = Regex.Match(text, @"Per[ií]odo\s*(?<d1>\d{4}-\d{2}-\d{2})\s*a\s*(?<d2>\d{4}-\d{2}-\d{2})", RegexOptions.IgnoreCase);
 if (per.Success) s.Period = $"{per.Groups["d1"].Value} a {per.Groups["d2"].Value}";
 s.IBAN = Regex.Match(text, @"IBAN\s*(?<v>PT\d{23})", RegexOptions.IgnoreCase).Groups["v"].Value;
 s.NIB = Regex.Match(text, @"NIB\s*(?<v>\d{21})", RegexOptions.IgnoreCase).Groups["v"].Value;
 s.SWIFT = Regex.Match(text, @"SWIFT(?:/BIC)?\s*(?<v>\S+)", RegexOptions.IgnoreCase).Groups["v"].Value;

 var prev = Regex.Match(text, @"Saldo\s+anterior\s*(?<v>-?[\d\.,]+)", RegexOptions.IgnoreCase);
 if (prev.Success) s.PreviousBalance = ParsePtDecimal(prev.Groups["v"].Value);

 // Use last Saldo contabilístico in DO section: pick the last occurrence
 var mAll = Regex.Matches(text, @"Saldo\s+contabil[íi]stico\s*(?<v>-?[\d\.,]+)", RegexOptions.IgnoreCase);
 if (mAll.Count >0)
 {
 s.FinalBalance = ParsePtDecimal(mAll[^1].Groups["v"].Value);
 }

 return s;
 }

 public (List<Loan> loans, List<LoanPayment> payments) ParseLoansAndPayments(string text)
 {
 var loans = new List<Loan>();
 var payments = new List<LoanPayment>();
 if (string.IsNullOrWhiteSpace(text)) return (loans, payments);

 var opts = RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant;
 var blockRx = new Regex(@"Cr[eé]dito\s*a\s*habita[cç][aã]o(?<block>.*?)(?:Cart[oõ]es|Autoriza[cç][oõ]es|$)", opts);
 var block = blockRx.Match(text);
 if (!block.Success) return (loans, payments);
 var b = block.Groups["block"].Value;

 var loan = new Loan
 {
 LoanType = "Crédito à habitação"
 };

 var cv = Regex.Match(b, @"Valor\s*Contratado\s*(?<v>[\d\.,]+)", opts);
 if (cv.Success) loan.ContractedValue = ParsePtDecimal(cv.Groups["v"].Value);
 var cd = Regex.Match(b, @"Data\s*Contrata[cç][aã]o\s*(?<d>\d{4}-\d{2}-\d{2})", opts);
 if (cd.Success && DateTime.TryParse(cd.Groups["d"].Value, out var cdd)) loan.ContractDate = cdd;
 var acc = Regex.Match(b, @"Conta\s*Débito\s*(?<v>[0-9\s\.]+)", opts);
 if (acc.Success) loan.AccountNumber = acc.Groups["v"].Value.Trim();
 var rate = Regex.Match(b, @"Indexante\s*(?<i>[\d\.,]+)\s*%\s*\+\s*Spread\s*(?<s>[\d\.,]+)\s*%\s*=\s*TANL\s*(?<t>[\d\.,]+)\s*%", opts);
 if (rate.Success)
 {
 loan.IndexRate = ParsePtDecimal(rate.Groups["i"].Value);
 loan.Spread = ParsePtDecimal(rate.Groups["s"].Value);
 loan.TANL = ParsePtDecimal(rate.Groups["t"].Value);
 }
 var outb = Regex.Match(b, @"Saldo\s*devedor\s*final\s*(?<v>[\d\.,]+)", opts);
 if (outb.Success) loan.OutstandingBalance = ParsePtDecimal(outb.Groups["v"].Value);

 // Monthly payment lines
 var capRx = new Regex(@"(?<d>\d{4}-\d{2}-\d{2})\s+(?<n>\d+)\s+COBRAN[ÇC]A\s+DE\s+CAPITAL\s+(?<v>[\d\.,]+)", opts);
 var jurRx = new Regex(@"(?<d>\d{4}-\d{2}-\d{2})\s+(?<n>\d+)\s+COBRAN[ÇC]A\s+DE\s+JUROS\s+(?<v>[\d\.,]+)", opts);
 foreach (Match m in capRx.Matches(b))
 {
 if (DateTime.TryParse(m.Groups["d"].Value, out var d))
 {
 payments.Add(new LoanPayment
 {
 MovementDate = d,
 InstallmentNumber = int.TryParse(m.Groups["n"].Value, out var n) ? n : null,
 CapitalAmount = ParsePtDecimal(m.Groups["v"].Value),
 Description = "COBRANCA DE CAPITAL",
 SourceLine = m.Value.Length >100 ? m.Value[..100] : m.Value
 });
 }
 }
 foreach (Match m in jurRx.Matches(b))
 {
 if (DateTime.TryParse(m.Groups["d"].Value, out var d))
 {
 payments.Add(new LoanPayment
 {
 MovementDate = d,
 InstallmentNumber = int.TryParse(m.Groups["n"].Value, out var n) ? n : null,
 InterestAmount = ParsePtDecimal(m.Groups["v"].Value),
 Description = "COBRANCA DE JUROS",
 SourceLine = m.Value.Length >100 ? m.Value[..100] : m.Value
 });
 }
 }

 // Agenda next payment
 var nextCap = Regex.Match(b, @"(?<d>\d{4}-\d{2}-\d{2})\s+(?<n>\d+)\s+VENCIMENTO\s+CAPITAL\s+(?<v>[\d\.,]+)", opts);
 if (nextCap.Success)
 {
 if (DateTime.TryParse(nextCap.Groups["d"].Value, out var nd)) loan.NextPaymentDate = nd;
 loan.NextCapitalPayment = ParsePtDecimal(nextCap.Groups["v"].Value);
 }
 var nextInt = Regex.Match(b, @"(?<d>\d{4}-\d{2}-\d{2})\s+(?<n>\d+)\s+VENCIMENTO\s+JUROS\s+(?<v>[\d\.,]+)", opts);
 if (nextInt.Success)
 {
 if (!loan.NextPaymentDate.HasValue && DateTime.TryParse(nextInt.Groups["d"].Value, out var nd)) loan.NextPaymentDate = nd;
 loan.NextInterestPayment = ParsePtDecimal(nextInt.Groups["v"].Value);
 }

 loans.Add(loan);
 return (loans, payments);
 }

 public List<DirectDebit> ParseDirectDebits(string text)
 {
 var list = new List<DirectDebit>();
 if (string.IsNullOrWhiteSpace(text)) return list;
 var opts = RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;
 var rowRx = new Regex(@"^(?<d>\d{4}-\d{2}-\d{2})\s+(?<name>.+?)\s+(?<ent>PT\d{2}\S+)\s+(?<auth>\d+)\s+(?<lim>(Sem Limite|[\d\.,]+))$", opts);

 foreach (Match m in rowRx.Matches(text))
 {
 var name = Regex.Replace(m.Groups["name"].Value, "\\s+", " ").Trim();
 var dd = new DirectDebit
 {
 CreationDate = DateTime.TryParse(m.Groups["d"].Value, out var d) ? d : null,
 CreditorName = name,
 EntityNumber = m.Groups["ent"].Value,
 AuthorizationNumber = m.Groups["auth"].Value,
 LimitValueRaw = m.Groups["lim"].Value
 };
 list.Add(dd);
 }
 return list;
 }

 public List<Card> ParseCards(string text)
 {
 var list = new List<Card>();
 if (string.IsNullOrWhiteSpace(text)) return list;
 var opts = RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;
 var rx = new Regex(@"CAIXA\s+CLASSIC\s*(?<num>\d{4}\*{8}\d{4})\s+(?<holder>.+?)\s*(?<val>[\d\.,]+)\s+(?<cur>EUR)", opts);
 foreach (Match m in rx.Matches(text))
 {
 list.Add(new Card
 {
 CardName = "CAIXA CLASSIC",
 MaskedNumber = m.Groups["num"].Value,
 Holder = m.Groups["holder"].Value.Trim(),
 CreditUsed = ParsePtDecimal(m.Groups["val"].Value),
 Currency = m.Groups["cur"].Value
 });
 }
 return list;
 }

 private static decimal? ParsePtDecimal(string s)
 {
 if (string.IsNullOrWhiteSpace(s)) return null;
 s = s.Trim().Replace(".", "").Replace(",", ".");
 return decimal.TryParse(s, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : (decimal?)null;
 }
 }
}
