using Newtonsoft.Json;
using PATHServer.BDD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplicationAPI;

namespace PATHServer
{
    public class PathTools
    {
        public static string GetJsonResponse( object tata, string message)
        {

            object rt = new
            {
                Object = tata,
                Message = message
            };

            return JsonConvert.SerializeObject(rt);
        }

        public static string GetJsonResponse( string message)
        {

            object rt = new
            {
                Message = message
            };

            return JsonConvert.SerializeObject(rt);
        }
    }
}
