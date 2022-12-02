using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PATHServer.BDD.Models
{
    public class DataInfo
    {
        [Key]
        public int di_id { get; set; }

        [Required]
        [MaxLength(255)]
        public string di_name { get; set; }

    }
}
