using PATHServer.BDD.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PATHServer.ModelParser
{
    public class DataHistoryParsed
    {
        public DataHistoryParsed(string dh_value, int node_id, DateTime dh_date)
        {
            this.dh_value = dh_value;
            this.node_id = node_id;
            this.dh_date = dh_date;
        }

        public string dh_value { get; private set; }

        public int node_id { get; private set; }

        public DateTime dh_date { get; private set; }

        public static DataHistoryParsed CreateFromModel(DataHistory dh)
        {
            string val;
            if(dh is DataHistoryBool dataHistoryBool)
            {
                val = dataHistoryBool.dh_bool_value ? "1" : "0";
            }
            else if(dh is DataHistoryDate dataHistoryDate)
            {
                val = dataHistoryDate.dh_date_value.ToString("G");
            }
            else if (dh is DataHistoryDouble historyDouble)
            {
                val = historyDouble.dh_double_value.ToString(CultureInfo.InvariantCulture);
            }
            else if (dh is DataHistoryInt historyInt)
            {
                val = historyInt.dh_int_value.ToString();
            }
            else if(dh is DataHistoryString historyString)
            {
                val = historyString.dh_string_value;
            }
            else
            {
                val = string.Empty;
            }

            return new DataHistoryParsed(val, dh.node_id, dh.dh_date);
        }
    }
}
