using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PATHServer.BDD.Models
{
    public class ActionHistory
    {
        [Key]
        public int ah_id { get; set; }

        [Required]
        public DateTime ah_date { get; set; }

        [Required]
        [ForeignKey("ActionHistoryInfo")]
        public int ahi_id { get; set; }

        [Required]
        [ForeignKey("PATHUser")]
        public int pu_id { get; set; }
    }
}
