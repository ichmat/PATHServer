using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PATHServer.BDD.Models
{
    public class ActionTrigger
    {
        [Key]
        public int ah_id { get; set; }


        [Required]
        public int act_type_data { get; set; }

        public InfoTypeData ActTypeData { get => (InfoTypeData)act_type_data; }


        [Required]
        public string act_name { get; set; }
    }
}
