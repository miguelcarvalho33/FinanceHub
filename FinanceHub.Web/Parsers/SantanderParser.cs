using System.Globalization;
using System.Text.RegularExpressions;
using FinanceHub.Core.Models;
using FinanceHub.Core.Parsing;

namespace FinanceHub.Web.Parsers
{
    public class SantanderParser : IPdfParser
    {
        public string BankName => "Santander";

        public bool CanParse(string text)
        {
            // A verificação pode ser simples, apenas para identificar o banco.
            return text.Contains("Santander", StringComparison.OrdinalIgnoreCase);
        }

        public List<Transaction> Parse(string text)
        {
            var transactions = new List<Transaction>();
            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var currentYear = DateTime.Now.Year;

            // Regex TRADUZIDA DIRETAMENTE do teu script Python.
            // Procura: DD-MM (espaços) DD-MM (espaços) Descrição (espaços) VALOR (espaços) SALDO
            var regex = new Regex(@"^(\d{2}-\d{2})\s+(\d{2}-\d{2})\s+(.*?)\s+(-?[\d\.,]+,\d{2})\s+(-?[\d\.,]+,\d{2})$");

            foreach (var line in lines)
            {
                var match = regex.Match(line.Trim());
                if (match.Success)
                {
                    try
                    {
                        var description = match.Groups[3].Value.Trim();

                        // Lógica de limpeza de valores do Python, traduzida para C#:
                        var amountString = match.Groups[4].Value.Trim();
                        var normalizedAmountString = amountString.Replace(".", "").Replace(",", ".");

                        var transaction = new Transaction
                        {
                            MovementDate = ParseDate(match.Groups[1].Value, currentYear),
                            ValueDate = ParseDate(match.Groups[2].Value, currentYear),
                            OriginalDescription = description,
                            Bank = this.BankName,
                            Amount = decimal.Parse(normalizedAmountString, CultureInfo.InvariantCulture) // Usamos InvariantCulture porque o formato agora é universal (ex: "123.45")
                        };
                        transactions.Add(transaction);
                    }
                    catch (Exception ex)
                    {
                        // Se uma linha falhar, logamos e continuamos para a próxima
                        Console.WriteLine($"AVISO [Santander]: Linha ignorada por erro de formato: '{line}'. Erro: {ex.Message}");
                    }
                }
            }
            return transactions;
        }

        private DateTime ParseDate(string dateStr, int year)
        {
            return DateTime.ParseExact($"{dateStr}-{year}", "dd-MM-yyyy", new CultureInfo("pt-PT"));
        }
    }
}