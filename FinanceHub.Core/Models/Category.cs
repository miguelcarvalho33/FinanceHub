using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FinanceHub.Core.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<DescriptionRule> DescriptionRules { get; set; } = new List<DescriptionRule>();
        public ICollection<MbwayRule> MbwayRules { get; set; } = new List<MbwayRule>();
    }
}
