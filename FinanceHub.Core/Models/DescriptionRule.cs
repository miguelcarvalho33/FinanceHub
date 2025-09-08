using System.ComponentModel.DataAnnotations;

namespace FinanceHub.Core.Models
{
    public class DescriptionRule
    {
        [Key]
        public int Id { get; set; }
        public string TextToFind { get; set; }
        public string CleanDescription { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
