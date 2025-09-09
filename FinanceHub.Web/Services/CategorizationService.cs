using FinanceHub.Core.Models;
using FinanceHub.Web.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace FinanceHub.Web.Services
{
    public class CategorizationService
    {
        // Listas privadas para guardar as regras em memória para acesso rápido
        private readonly List<DescriptionRule> _descriptionRules;
        private readonly List<MbwayRule> _mbwayRules;
        private readonly ILogger<CategorizationService> _logger;

        public CategorizationService(FinanceDbContext dbContext, ILogger<CategorizationService> logger)
        {
            _logger = logger;

            // Carrega TODAS as regras da base de dados quando o serviço é criado.
            // O Include() garante que a informação da Categoria associada vem junto.
            _descriptionRules = dbContext.DescriptionRules.Include(r => r.Category).ToList();
            _mbwayRules = dbContext.MbwayRules.Include(r => r.Category).ToList();

            _logger.LogInformation("CategorizationService inicializado com {DescriptionRulesCount} regras de descrição e {MbwayRulesCount} regras de MBWay.", _descriptionRules.Count, _mbwayRules.Count);
        }

        /// <summary>
        /// Processa uma única transação, aplicando as regras de categorização.
        /// </summary>
        public void ProcessTransaction(Transaction transaction)
        {
            // Primeiro, tentamos as regras mais específicas (MBWay)
            if (ApplyMbwayRules(transaction))
            {
                return; // Se uma regra MBWay foi aplicada, o trabalho está feito.
            }

            // Se nenhuma regra MBWay correspondeu, tentamos as regras de descrição
            if (ApplyDescriptionRules(transaction))
            {
                return; // Se uma regra de descrição foi aplicada, o trabalho está feito.
            }

            // Se chegarmos aqui, nenhuma regra correspondeu.
            _logger.LogWarning("Nenhuma regra de categorização encontrada para a descrição: '{OriginalDescription}'", transaction.OriginalDescription);

            // Atribuímos valores padrão para não deixar os campos vazios
            transaction.CleanDescription = transaction.OriginalDescription;
            // O CategoryId continuará nulo, para sabermos o que falta categorizar.
        }

        private bool ApplyDescriptionRules(Transaction transaction)
        {
            // Percorre todas as regras de descrição
            foreach (var rule in _descriptionRules)
            {
                // Verifica se o "Texto a Procurar" da regra existe na descrição original (ignorando maiúsculas/minúsculas)
                if (transaction.OriginalDescription.Contains(rule.TextToFind, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("Regra de descrição encontrada: '{TextToFind}' -> Categoria '{CategoryName}'", rule.TextToFind, rule.Category.Name);
                    transaction.Category = rule.Category;
                    transaction.CategoryId = rule.CategoryId;
                    transaction.CleanDescription = rule.CleanDescription;
                    return true; // Sucesso, regra aplicada
                }
            }
            return false; // Nenhuma regra de descrição correspondeu
        }

        // Dentro da classe CategorizationService
        private bool ApplyMbwayRules(Transaction transaction)
        {
            var mbwayRegex = new Regex(@"P/XXXXX(\d{4})");
            var match = mbwayRegex.Match(transaction.OriginalDescription);

            if (match.Success)
            {
                var last4Digits = match.Groups[1].Value; // Captura apenas os 4 dígitos
                                                         // Procura se temos uma regra para estes 4 dígitos
                var rule = _mbwayRules.FirstOrDefault(r => r.PhoneNumberSuffix == last4Digits);
                if (rule != null)
                {
                    _logger.LogInformation("Regra MBWay encontrada: Suffix '{Suffix}' -> '{ContactName}'", rule.PhoneNumberSuffix, rule.ContactName);
                    transaction.Category = rule.Category;
                    transaction.CategoryId = rule.CategoryId;
                    transaction.CleanDescription = $"MBWay - {rule.ContactName}";
                    return true;
                }
            }
            return false;
        }
    }
}