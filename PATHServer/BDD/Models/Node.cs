using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PATHServer.BDD.Models
{
    public class Node
    {
        [Key]
        public int node_id { get; set; }

        [Required]
        [MaxLength(50)]
        public string node_name { get; set; }

        [Required]
        [DisplayName("node_type_data")]
        public int node_type_data { get; set; }

        public InfoTypeData NodeTypeData { get => (InfoTypeData)node_type_data; }
    }
}
