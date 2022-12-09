using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PATHServer.BDD.Models
{
    public class PATHUser
    {
        [Key]
        public int pu_id { get; set; }

        [Required]
        [MaxLength(500)]
        public string pu_email { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string pu_name { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string pu_surname { get; set; }

        [Required]
        [MaxLength(255)]
        public string pu_password { get; set; }

        [Required]
        public bool pu_admin { get; set; }

    }
}
