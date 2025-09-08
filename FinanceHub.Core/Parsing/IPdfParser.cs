using FinanceHub.Core.Models;

namespace FinanceHub.Core.Parsing
{
    public interface IPdfParser
    {
        string BankName { get; }
        bool CanParse(string text);
        List<Transaction> Parse(string text);
    }
}
