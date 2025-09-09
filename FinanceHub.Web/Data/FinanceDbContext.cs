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
    }
}