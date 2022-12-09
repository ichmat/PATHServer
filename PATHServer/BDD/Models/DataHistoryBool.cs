using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PATHServer.BDD.Models
{
    public class DataHistoryBool : DataHistory
    {
        [Required]
        public bool dh_bool_value { get; set; }
    }
}
