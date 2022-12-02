using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PATHServer.BDD.Models
{
    public class SensorData
    {
        [Key]
        public int SensorId { get; set; }

        [Required]
        [MaxLength(128)]
        public string Data { get; set; }

        [Required]
        public DateTime DateTimeAdd { get; set; }
    }
}
