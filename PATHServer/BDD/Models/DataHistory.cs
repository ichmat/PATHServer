using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PATHServer.BDD.Models
{
    public class DataHistory
    {
        [Key]
        public int dh_id { get; set; }

        [Required]
        public DateTime dh_date { get; set; }

        [Required]
        [ForeignKey("Node")]
        public int node_id { get; set; }
    }
}
