using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PATHServer.BDD.Models
{
    public class KeyConnexion
    {
        [Key]
        [MaxLength(255)]
        public string key_id { get; set; }

        [Required]
        [MaxLength(255)]
        public DateTime key_created { get; set; }

        [Required]
        public DateTime key_lastUpdated { get; set; }

        [Required]
        public int key_quota { get; set; }

        [Required]
        public DateTime key_quotaRefresh { get; set; }

        //[key]
      //  [FOREIGN KEY]
        public int pu_id { get; set; }
    }
}
