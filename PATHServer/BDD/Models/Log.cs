using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PATHServer.BDD.Models
{
    public class Log
    {
        internal static readonly string[] TypeLog =
        {
            "INFO",
            "WARNING",
            "FAIL",
            "ERROR",
            "FATAL",
            "SUCCESS"
        };

        public static Log GenerateLog(string quoi, TypeLOG type, int? qui)
        {
            Log log = new Log();
            log.log_type = GetTypeLog(type);
            log.log_what = quoi;
            log.log_who = qui;
            log.log_id = Guid.NewGuid().ToString();
            log.log_when = DateTime.Now.ToUniversalTime();
            return log;
        }

        private static string GetTypeLog(TypeLOG type)
        {
            return TypeLog[(int)type];
        }

        [Key]
        public string log_id { get; set; }

        public int? log_who { get; set; }

        [Required]
        public string log_what { get; set;}

        [Required]
        public string log_type { get; set;}

        [Required]
        public DateTime log_when { get; set;}
    }

    public enum TypeLOG
    {
        INFO = 0,
        WARNING = 1,
        FAIL = 2,
        ERROR = 3,
        FATAL = 4,
        SUCCESS = 5
    }
}
