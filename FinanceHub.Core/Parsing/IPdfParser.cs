using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
