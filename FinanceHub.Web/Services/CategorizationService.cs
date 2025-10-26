using System.Text.RegularExpressions;
using FinanceHub.Core.Models;
using FinanceHub.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Web.Services
{
    public class CategorizationService
    {
        private readonly List<DescriptionRule> _descriptionRules;
        private readonly List<MbwayRule> _mbwayRules;
        private readonly ILogger<CategorizationService> _logger;
        private readonly FinanceDbContext _dbContext; // Precisamos do DbContext aqui
        private readonly Dictionary<string, int> _mbwayConfigRules;

        public CategorizationService(
            FinanceDbContext dbContext,
            ILogger<CategorizationService> logger,
            IConfiguration config)
        {
            _logger = logger;
            _dbContext = dbContext;

            _descriptionRules = _dbContext.DescriptionRules.Include(r => r.Category).ToList();
            _mbwayRules = _dbContext.MbwayRules.Include(r => r.Category).ToList();

            _mbwayConfigRules = config.GetSection("MbwayRules")
                                      .Get<Dictionary<string, int>>() ?? new();

            _logger.LogInformation(
                "CategorizationService inicializado com {DescriptionRulesCount} regras de descrição, {MbwayRulesCount} regras de MBWay DB e {MbwayConfigRulesCount} regras de MBWay Config.",
                _descriptionRules.Count,
                _mbwayRules.Count,
                _mbwayConfigRules.Count
            );
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

            _logger.LogWarning("Nenhuma regra de categorização encontrada para a descrição: '{OriginalDescription}'", transaction.OriginalDescription);
            transaction.CleanDescription = transaction.OriginalDescription;
        }

        private bool ApplyDescriptionRules(Transaction transaction)
        {
            foreach (var rule in _descriptionRules)
            {
                if (transaction.OriginalDescription.Contains(rule.TextToFind, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("Regra de descrição encontrada: '{TextToFind}' -> Categoria '{CategoryName}'", rule.TextToFind, rule.Category.Name);
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
            bool matched = false;

            // --- 1. Regra MANUAL pelo sufixo ---
            var mbwaySuffixRegex = new Regex(@"P/XXXXX(\d{4})");
            var suffixMatch = mbwaySuffixRegex.Match(transaction.OriginalDescription);

            if (suffixMatch.Success)
            {
                var last4Digits = suffixMatch.Groups[1].Value;
                var rule = _mbwayRules.FirstOrDefault(r => r.PhoneNumberSuffix == last4Digits);
                if (rule != null)
                {
                    _logger.LogInformation("Regra MBWay MANUAL encontrada: Suffix '{Suffix}' -> '{ContactName}'", rule.PhoneNumberSuffix, rule.ContactName);
                    transaction.Category = rule.Category;
                    transaction.CategoryId = rule.CategoryId;
                    transaction.CleanDescription = $"MBWay - {rule.ContactName}";
                    matched = true;
                }
            }

            // --- 2. Contacto na BD pelo número completo ---
            if (!matched)
            {
                var mbwayPhoneRegex = new Regex(@"(\d{9})");
                var phoneMatch = mbwayPhoneRegex.Match(transaction.OriginalDescription);

                if (phoneMatch.Success)
                {
                    var phoneNumber = phoneMatch.Value;
                    var contact = _dbContext.Contacts.FirstOrDefault(c => c.PhoneNumber == phoneNumber);

                    if (contact != null)
                    {
                        _logger.LogInformation("Contacto AUTOMÁTICO encontrado: '{PhoneNumber}' -> '{ContactName}'", phoneNumber, contact.Name);

                        var transferCategory = _dbContext.Categories.FirstOrDefault(c => c.Name == "Transferências");
                        if (transferCategory != null)
                        {
                            transaction.Category = transferCategory;
                            transaction.CategoryId = transferCategory.Id;
                        }
                        transaction.CleanDescription = $"MBWay - {contact.Name}";
                        matched = true;
                    }
                }
            }

            // --- 3. OVERRIDE PELO CONFIG (tem prioridade total) ---
            if (transaction.Amount != 0 && _mbwayConfigRules.Any())
            {
                var descricoes = _mbwayConfigRules
                    .Where(r => r.Value == (int)transaction.Amount)
                    .Select(r => r.Key)
                    .ToList();

                if (descricoes.Any())
                {
                    var descricao = string.Join(", ", descricoes);

                    // Forçar categoria "Habitação" (Id = 2)
                    var habitacaoCategory = _dbContext.Categories.FirstOrDefault(c => c.Id == 2);

                    _logger.LogInformation("OVERRIDE CONFIG: Valor {Amount} -> '{Descricao}', Categoria = Habitação", transaction.Amount, descricao);
                    transaction.CleanDescription = descricao;
                    if (habitacaoCategory != null)
                    {
                        transaction.Category = habitacaoCategory;
                        transaction.CategoryId = habitacaoCategory.Id;
                    }

                    return true;
                }
            }

            return matched;
        }
    }
}