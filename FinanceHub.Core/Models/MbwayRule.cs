using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FinanceHub.Core.Models
{
    public class MbwayRule
    {
        [Key]
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactName { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
