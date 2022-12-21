using PATHServer.BDD.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PATHServer.ModelParser
{
    public class DataLiveParsed
    {
        public int dl_id { get; private set; }

        public string dl_name { get; private set; }

        public DataLiveParsed(int dl_id, string dl_name)
        {
            this.dl_id = dl_id;
            this.dl_name = dl_name;
        }

        public static DataLiveParsed CreateFromModel(DataLive model)
        {
            return new DataLiveParsed(
                model.dl_id,
                model.dl_name
                );
        }
    }
}
