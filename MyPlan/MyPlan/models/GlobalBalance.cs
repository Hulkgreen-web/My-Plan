using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPlan.models
{
    public class GlobalBalance
    {
        [Key]
        public int Id { get; set; } = 1; // Toujours le même ID

        [Required]
        public decimal Balance { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
