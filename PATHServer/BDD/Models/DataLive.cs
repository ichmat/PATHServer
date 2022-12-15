using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace PATHServer.BDD.Models
{
    public class DataLive
    {
        [Key]
        public int dl_id { get; set; }

        [Required]
        [MaxLength(255)]
        public string dl_name { get; set; }

        public int dl_val_int { get; set; }

        public double dl_val_double { get; set; }

        [MaxLength(255)]
        public string dl_val_string { get; set; }

        public DateTime dl_val_datetime { get; set; }

        public bool dl_val_bool { get; set; }
    }
}
