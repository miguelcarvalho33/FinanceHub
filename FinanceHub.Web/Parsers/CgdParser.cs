using FinanceHub.Core.Models;
using FinanceHub.Core.Parsing;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FinanceHub.Web.Parsers
{
    public class CgdParser : IPdfParser
    {
        public string BankName => "Caixa Geral de Depósitos";

        public bool CanParse(string text)
        {
            return text.Contains("Caixa Geral de Depósitos", StringComparison.OrdinalIgnoreCase);
        }

        public List<Transaction> Parse(string text)
        {
            var transactions = new List<Transaction>();
            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // Regex TRADUZIDA DIRETAMENTE do teu script Python.
            // Procura: YYYY-MM-DD (espaços) YYYY-MM-DD (espaços) Descrição (espaços) VALOR (espaços) SALDO
            var regex = new Regex(@"^(\d{4}-\d{2}-\d{2})\s+(\d{4}-\d{2}-\d{2})\s+(.*?)\s+(-?[\d\.,]+,\d{2})\s+(-?[\d\.,]+,\d{2})$");

            foreach (var line in lines)
            {
                Match match = regex.Match(line.Trim());
                if (match.Success)
                {
                    try
                    {
                        // Lógica de limpeza de valores do Python, traduzida para C#:
                        var amountString = match.Groups[4].Value.Trim();
                        var normalizedAmountString = amountString.Replace(".", "").Replace(",", ".");

                        var transaction = new Transaction
                        {
                            MovementDate = DateTime.ParseExact(match.Groups[1].Value, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                            ValueDate = DateTime.ParseExact(match.Groups[2].Value, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                            OriginalDescription = match.Groups[3].Value.Trim(),
                            Amount = decimal.Parse(normalizedAmountString, CultureInfo.InvariantCulture),
                            Bank = this.BankName
                        };
                        transactions.Add(transaction);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"AVISO [CGD]: Linha ignorada por erro de formato: '{line}'. Erro: {ex.Message}");
                    }
                }
            }
            return transactions;
        }
    }
}