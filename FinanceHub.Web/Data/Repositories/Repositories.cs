using FinanceHub.Core.Application.Interfaces;
using FinanceHub.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Web.Data.Repositories
{
 public class BankStatementRepository : IBankStatementRepository
 {
 private readonly FinanceDbContext _db;
 public BankStatementRepository(FinanceDbContext db) => _db = db;

 public async Task<BankStatement> AddOrGetAsync(BankStatement statement, CancellationToken ct = default)
 {
 var existing = await _db.BankStatements.FirstOrDefaultAsync(s =>
 (s.Bank == statement.Bank && s.StatementNumber == statement.StatementNumber) ||
 (s.Bank == statement.Bank && s.IBAN == statement.IBAN && s.Period == statement.Period), ct);
 if (existing != null) return existing;
 _db.BankStatements.Add(statement);
 await _db.SaveChangesAsync(ct);
 return statement;
 }

 public async Task LinkTransactionsAsync(int statementId, IEnumerable<Transaction> transactions, CancellationToken ct = default)
 {
 foreach (var t in transactions)
 {
 t.BankStatementId = statementId;
 }
 await _db.SaveChangesAsync(ct);
 }
 }

 public class TransactionRepository : ITransactionRepository
 {
 private readonly FinanceDbContext _db;
 public TransactionRepository(FinanceDbContext db) => _db = db;

 public async Task AddNewAsync(IEnumerable<Transaction> transactions, CancellationToken ct = default)
 {
 if (!transactions.Any()) return;
 _db.Transactions.AddRange(transactions);
 await _db.SaveChangesAsync(ct);
 }
 public Task<bool> ExistsByHashAsync(string hash, CancellationToken ct = default)
 => _db.Transactions.AnyAsync(t => t.Hash == hash, ct);
 }
}
