// FinanceHub.Web/Data/FinanceDbContext.cs

using FinanceHub.Core.Models;       // <-- Verifique este using
using Microsoft.EntityFrameworkCore; // <-- Verifique este using

namespace FinanceHub.Web.Data // <-- A LINHA MAIS IMPORTANTE!
{
    public class FinanceDbContext : DbContext
    {
        public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options) { }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<DescriptionRule> DescriptionRules { get; set; }
        public DbSet<MbwayRule> MbwayRules { get; set; }
    }
}