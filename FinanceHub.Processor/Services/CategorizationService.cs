using FinanceHub.Core.Models;
using FinanceHub.Processor.Data;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Processor.Services
{
    public class CategorizationService
    {
        private readonly List<DescriptionRule> _descriptionRules;
        private readonly List<MbwayRule> _mbwayRules;

        public CategorizationService(FinanceDbContext dbContext)
        {
            _descriptionRules = dbContext.DescriptionRules.Include(r => r.Category).ToList();
            _mbwayRules = dbContext.MbwayRules.Include(r => r.Category).ToList();
        }

        public void ProcessTransaction(Transaction transaction)
        {
            if (ApplyMbwayRules(transaction))
            {
                return;
            }

            if (ApplyDescriptionRules(transaction))
            {
                return;
            }

            transaction.CleanDescription = transaction.OriginalDescription;
        }

        private bool ApplyDescriptionRules(Transaction transaction)
        {
            foreach (var rule in _descriptionRules)
            {
                if (transaction.OriginalDescription.Contains(rule.TextToFind, StringComparison.OrdinalIgnoreCase))
                {
                    transaction.Category = rule.Category;
                    transaction.CategoryId = rule.CategoryId;
                    transaction.CleanDescription = rule.CleanDescription;
                    return true;
                }
            }
            return false;
        }

        private bool ApplyMbwayRules(Transaction transaction)
        {
            var mbwayRegex = new Regex(@"9[1236]\d{7}");
            var match = mbwayRegex.Match(transaction.OriginalDescription);

            if (match.Success)
            {
                var phoneNumber = match.Value;
                foreach (var rule in _mbwayRules)
                {
                    if (rule.PhoneNumber == phoneNumber)
                    {
                        transaction.Category = rule.Category;
                        transaction.CategoryId = rule.CategoryId;
                        transaction.CleanDescription = $"MBWay - {rule.ContactName}";
                        return true;
                    }
                }
            }
            return false;
        }
    }
}