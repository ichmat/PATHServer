using PATHServer.BDD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PATHServer.ModelParser
{
    public class ActionTriggerParsed
    {
        public int ah_id { get; private set; }
        public string act_name { get; private set; }
        public string act_type_data { get; private set; }

        public ActionTriggerParsed(int ah_id, string act_name, string act_type_data)
        {
            this.ah_id = ah_id;
            this.act_name = act_name;
            this.act_type_data = act_type_data;
        }

        public static ActionTriggerParsed CreateFromModel(ActionTrigger model)
        {
            return new ActionTriggerParsed(
                model.ah_id,
                model.act_name,
                model.ActTypeData.ToString()
                );
        }
    }
}
