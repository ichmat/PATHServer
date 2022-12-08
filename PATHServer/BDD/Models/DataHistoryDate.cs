using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PATHServer.BDD.Models
{
    public class DataHistoryDate
    {
        [Required]
        [ForeignKey("dh_id")]
        public int dh_id { get; set; }

        [Required]
        public DateTime dh_date_value { get; set; }
    }
}
