using FinanceHub.Core.Models;
using FinanceHub.Core.Parsing;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FinanceHub.Processor.Parsers
{
    public class CgdParser : IPdfParser
    {
        public string BankName => "Caixa Geral de Depósitos";

        public bool CanParse(string text)
        {
            return text.Contains("Caixa Geral de Depósitos", StringComparison.OrdinalIgnoreCase) &&
                   text.Contains("Depósitos à Ordem", StringComparison.OrdinalIgnoreCase);
        }

        public List<Transaction> Parse(string text)
        {
            var transactions = new List<Transaction>();

            var regex = new Regex(
                @"^(\d{4}-\d{2}-\d{2})\s+" +     // 1: Data Mov (ex: 2025-05-02)
                @"(\d{4}-\d{2}-\d{2})\s+" +     // 2: Data Valor (ex: 2025-05-02)
                @"(.+?)\s+" +                     // 3: Descrição
                @"(-?[\d.,]+,\d{2})\s+" +         // 4: VALOR (com sinal opcional '-')
                @"([\d.,]+,\d{2})$",               // 5: Saldo
                RegexOptions.Multiline);

            var transactionText = GetTransactionBlock(text);

            var matches = regex.Matches(transactionText);

            foreach (Match match in matches)
            {
                try
                {
                    var transaction = new Transaction
                    {
                        MovementDate = ParseDate(match.Groups[1].Value),
                        ValueDate = ParseDate(match.Groups[2].Value),
                        OriginalDescription = match.Groups[3].Value.Trim(),
                        Bank = this.BankName,
                        Amount = ParseDecimal(match.Groups[4].Value)
                    };
                    transactions.Add(transaction);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"AVISO [CGD]: Linha ignorada por erro de formato: '{match.Value}'. Erro: {ex.Message}");
                }
            }
            return transactions;
        }

        private string GetTransactionBlock(string originalText)
        {
            var header = "Descrição";
            var headerIndex = originalText.IndexOf(header, StringComparison.OrdinalIgnoreCase);
            if (headerIndex == -1) return originalText;

            return originalText.Substring(headerIndex + header.Length);
        }

        private DateTime ParseDate(string dateStr)
        {
            return DateTime.ParseExact(dateStr.Trim(), "yyyy-MM-dd", new CultureInfo("pt-PT"));
        }

        private decimal ParseDecimal(string decimalStr)
        {
            return decimal.Parse(decimalStr.Trim(), new CultureInfo("pt-PT"));
        }
    }
}