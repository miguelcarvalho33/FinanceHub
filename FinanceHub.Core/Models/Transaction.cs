using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FinanceHub.Core.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public DateTime MovementDate { get; set; }
        public DateTime ValueDate { get; set; }
        public string OriginalDescription { get; set; }
        public decimal Amount { get; set; }
        public string Bank { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public string? CleanDescription { get; set; }
        public string Hash { get; set; }
    }
}