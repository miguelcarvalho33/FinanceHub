using FinanceHub.Core.Models;

namespace FinanceHub.Core.Application.Interfaces
{
 /// <summary>
 /// Abstraction for persisting statements and related aggregates.
 /// </summary>
 public interface IBankStatementRepository
 {
 Task<BankStatement> AddOrGetAsync(BankStatement statement, CancellationToken ct = default);
 Task LinkTransactionsAsync(int statementId, IEnumerable<Transaction> transactions, CancellationToken ct = default);
 }

 /// <summary>
 /// Abstraction for persisting transactions with de-duplication.
 /// </summary>
 public interface ITransactionRepository
 {
 Task AddNewAsync(IEnumerable<Transaction> transactions, CancellationToken ct = default);
 Task<bool> ExistsByHashAsync(string hash, CancellationToken ct = default);
 }
}
