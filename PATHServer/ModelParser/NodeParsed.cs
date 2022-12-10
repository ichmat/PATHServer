using PATHServer.BDD.Models;
using PATHServer.ModelParser;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WebApplicationAPI
{
    public class NodeParsed
    {
        public int node_id { get; private set; }

        public string node_name { get; private set; }

        public string node_type_data { get; private set; }


        public NodeParsed(int node_id, string node_name, string node_type_data)
        {
            this.node_id = node_id;
            this.node_name = node_name;
            this.node_type_data = node_type_data;
        }

        public static NodeParsed CreateFromModel(Node model)
        {
            return new NodeParsed(
                model.node_id,
                model.node_name,
                model.InfoTypeData.ToString()
                );
        }
    }
}
