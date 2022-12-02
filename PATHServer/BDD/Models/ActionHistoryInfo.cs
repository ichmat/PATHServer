using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace PATHServer.BDD.Models
{
    public class ActionHistoryInfo
    {
        [Key]
        public int ahi_id { get; set; }

        [Required]
        [MaxLength(255)]
        public string ahi_name { get; set; }
    }
}
