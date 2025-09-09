// FinanceHub.Core/Models/MbwayRule.cs
using System.ComponentModel.DataAnnotations;

namespace FinanceHub.Core.Models
{
    public class MbwayRule
    {
        [Key]
        public int Id { get; set; }
        public string PhoneNumberSuffix { get; set; }
        public string ContactName { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}