using FinanceHub.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Web.Data
{
    public class FinanceDbContext : DbContext
    {
        public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options) { }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<DescriptionRule> DescriptionRules { get; set; }
        public DbSet<MbwayRule> MbwayRules { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<BankStatement> BankStatements { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<LoanPayment> LoanPayments { get; set; }
        public DbSet<DirectDebit> DirectDebits { get; set; }
        public DbSet<Card> Cards { get; set; }
    }
}