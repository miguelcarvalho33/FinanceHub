using FinanceHub.Core.Models;
using FinanceHub.Core.Parsing;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FinanceHub.Processor.Parsers
{
    public class SantanderParser : IPdfParser
    {
        public string BankName => "Santander";

        public bool CanParse(string text)
        {
            return text.Contains("Santander", StringComparison.OrdinalIgnoreCase) &&
                   text.Contains("Detalhe de Movimentos da Conta à Ordem", StringComparison.OrdinalIgnoreCase);
        }

        public List<Transaction> Parse(string text)
        {
            var transactions = new List<Transaction>();

            var regex = new Regex(
                @"^(\d{2}-\d{2})\s+" +             // 1: Data Mov (ex: 03-01)
                @"(\d{2}-\d{2})\s+" +             // 2: Data Valor (ex: 03-01)
                @"(.+?)\s+" +                     // 3: Descrição
                @"(-?[\d.,]+,\d{2})\s+" +         // 4: VALOR (sempre positivo no texto)
                @"([\d.,]+,\d{2})$",               // 5: Saldo
                RegexOptions.Multiline);

            var transactionText = GetTransactionBlock(text);
            var currentYear = DateTime.Now.Year;

            var matches = regex.Matches(transactionText);

            foreach (Match match in matches)
            {
                try
                {
                    var description = match.Groups[3].Value.Trim();
                    var amount = ParseDecimal(match.Groups[4].Value);

                    var transaction = new Transaction
                    {
                        MovementDate = ParseDate(match.Groups[1].Value, currentYear),
                        ValueDate = ParseDate(match.Groups[2].Value, currentYear),
                        OriginalDescription = description,
                        Bank = this.BankName,
                        Amount = amount
                    };
                    transactions.Add(transaction);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"AVISO [Santander]: Linha ignorada por erro de formato: '{match.Value}'. Erro: {ex.Message}");
                }
            }
            return transactions;
        }

        private string GetTransactionBlock(string originalText)
        {
            var header = "Descritivo do Movimento";
            var headerIndex = originalText.IndexOf(header, StringComparison.OrdinalIgnoreCase);
            if (headerIndex == -1) return originalText;

            return originalText.Substring(headerIndex + header.Length);
        }


        private DateTime ParseDate(string dateStr, int year)
        {
            var parts = dateStr.Split('-');
            var day = parts[0];
            var month = parts[1];

            string tempDateString = $"{year}-{month}-{day}";

            return DateTime.ParseExact(tempDateString, "yyyy-MM-dd", new CultureInfo("pt-PT"));
        }

        private decimal ParseDecimal(string decimalStr)
        {
            return decimal.Parse(decimalStr.Trim(), new CultureInfo("pt-PT"));
        }
    }
}