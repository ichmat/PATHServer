using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace PATHServer.BDD.Models
{
    public class Node
    {
        [Key]
        public int node_id { get; set; }

        [Required]
        [MaxLength(50)]
        public string node_name { get; set; }
    }
}
