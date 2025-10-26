using System.Globalization;
using System.Text.RegularExpressions;
using FinanceHub.Core.Models;
using FinanceHub.Core.Parsing;

namespace FinanceHub.Web.Parsers
{
    public class SantanderParser : IPdfParser
    {
        public string BankName => "Banco Santander Totta";

        public bool CanParse(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            return text.Contains("Banco Santander Totta", StringComparison.OrdinalIgnoreCase)
                || text.Contains("BIC/SWIFT: TOTAPTPL", StringComparison.OrdinalIgnoreCase)
                || text.Contains("TOTAPTPL", StringComparison.OrdinalIgnoreCase)
                || text.Contains("EXTCON", StringComparison.OrdinalIgnoreCase);
        }

        public List<Transaction> Parse(string text)
        {
            var results = new List<Transaction>();
            if (string.IsNullOrWhiteSpace(text)) return results;

            // Infer year from statement period if available
            var year = InferYearFromPeriod(text) ?? DateTime.Now.Year;

            // Lines like:01-0502-05 DESCRIÇÃO -12,341.234,56
            var rx = new Regex(
                @"^(?<d1>\d{2}-\d{2})\s+(?<d2>\d{2}-\d{2})\s+(?<desc>.*?)\s+(?<amount>-?[\d\.,]+)\s+(?<balance>-?[\d\.,]+)$",
                RegexOptions.Multiline | RegexOptions.CultureInvariant);

            foreach (Match m in rx.Matches(text))
            {
                try
                {
                    var d1 = ParseDayMonth(m.Groups["d1"].Value, year);
                    var d2 = ParseDayMonth(m.Groups["d2"].Value, year);
                    var desc = m.Groups["desc"].Value.Trim();
                    var amount = ParsePtDecimal(m.Groups["amount"].Value) ?? 0m;

                    results.Add(new Transaction
                    {
                        MovementDate = d1,
                        ValueDate = d2,
                        OriginalDescription = desc,
                        Amount = amount,
                        Bank = BankName
                    });
                }
                catch
                {
                    // ignore malformed line
                }
            }

            return results;
        }

        public BankStatement ParseMetadata(string text)
        {
            var bs = new BankStatement
            {
                Bank = BankName
            };

            // Extrato Nº
            var extrato = Regex.Match(text, @"EXTRATO\s+N[º°]\s*(?<no>\d+)", RegexOptions.IgnoreCase);
            if (extrato.Success) bs.StatementNumber = extrato.Groups["no"].Value;

            // Conta Nº (optional capture)
            var conta = Regex.Match(text, @"CONTA\s+N[º°]\s*(?<acc>[\d\.]+)", RegexOptions.IgnoreCase);
            if (conta.Success) bs.ClientNumber = conta.Groups["acc"].Value;

            // IBAN
            var iban = Regex.Match(text, @"IBAN[:]?\s*(?<iban>PT\d{23})", RegexOptions.IgnoreCase);
            if (iban.Success) bs.IBAN = iban.Groups["iban"].Value;
            // NIB
            var nib = Regex.Match(text, @"N\.?I\.?B\.?\s*:?\s*(?<nib>[\d\s]{21,})", RegexOptions.IgnoreCase);
            if (nib.Success) bs.NIB = Regex.Replace(nib.Groups["nib"].Value, @"\s+", "");
            // SWIFT/BIC
            var swift = Regex.Match(text, @"BIC/SWIFT[:]?\s*(?<swift>\S+)", RegexOptions.IgnoreCase);
            if (swift.Success) bs.SWIFT = swift.Groups["swift"].Value;

            // Período (accept both "PERÍODO DE ... A ..." and "Período ... a ...")
            var per1 = Regex.Match(text, @"PER[ÍI]ODO\s+DE\s+(?<d1>\d{4}-\d{2}-\d{2})\s+A\s+(?<d2>\d{4}-\d{2}-\d{2})", RegexOptions.IgnoreCase);
            var per2 = Regex.Match(text, @"Per[íi]odo\s+(?<d1>\d{4}-\d{2}-\d{2})\s+a\s+(?<d2>\d{4}-\d{2}-\d{2})", RegexOptions.IgnoreCase);
            var per = per1.Success ? per1 : per2;
            if (per.Success)
            {
                var from = per.Groups["d1"].Value;
                var to = per.Groups["d2"].Value;
                bs.Period = $"{from} a {to}";
            }

            // Data de emissão
            var em = Regex.Match(text, @"DATA\s+DE\s+EMISS[ÃA]O[:]?\s*(?<d>\d{4}-\d{2}-\d{2})", RegexOptions.IgnoreCase);
            if (em.Success && DateTime.TryParse(em.Groups["d"].Value, out var ed)) bs.EmissionDate = ed;

            // Saldos
            var saldoIni = Regex.Match(text, @"Saldo\s+Inicial\s+(?:EUR\s+)?(?<v>-?[\d\.,]+)", RegexOptions.IgnoreCase);
            if (saldoIni.Success) bs.PreviousBalance = ParsePtDecimal(saldoIni.Groups["v"].Value);

            // Final balance: last number in last Saldo column match
            var saldoMovRx = new Regex(@"^(?<d1>\d{2}-\d{2})\s+(?<d2>\d{2}-\d{2})\s+.*?\s+-?[\d\.,]+\s+(?<saldo>-?[\d\.,]+)$", RegexOptions.Multiline);
            decimal? lastSaldo = null;
            foreach (Match m in saldoMovRx.Matches(text))
            {
                lastSaldo = ParsePtDecimal(m.Groups["saldo"].Value);
            }
            if (lastSaldo.HasValue) bs.FinalBalance = lastSaldo;

            // Savings summary optional
            var poupanca = Regex.Match(text, @"DEP\.REND\.POUPANCA\s+EUR\s+(?<v>[\d\.,]+)", RegexOptions.IgnoreCase);
            if (poupanca.Success) bs.SavingsBalance = ParsePtDecimal(poupanca.Groups["v"].Value);

            return bs;
        }

        private static int? InferYearFromPeriod(string text)
        {
            var per1 = Regex.Match(text, @"(\d{4})-\d{2}-\d{2}\s*[aA]\s*(\d{4})-\d{2}-\d{2}");
            if (per1.Success)
            {
                if (int.TryParse(per1.Groups[1].Value, out var y)) return y;
            }
            return null;
        }

        private static DateTime ParseDayMonth(string ddmm, int year)
        {
            return DateTime.ParseExact(ddmm + "-" + year.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
        }

        private static decimal? ParsePtDecimal(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            s = s.Trim().Replace(".", "").Replace(",", ".");
            return decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var v) ? v : (decimal?)null;
        }
    }
}