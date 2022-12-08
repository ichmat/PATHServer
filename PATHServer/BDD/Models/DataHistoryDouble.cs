using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PATHServer.BDD.Models
{
    public class DataHistoryDouble : DataHistory
    {
        /*[Required]
        [ForeignKey("dh_id")]
        public int dh_id { get; set; }*/

        [Required]
        public double dh_double_value { get; set; }
    }
}
