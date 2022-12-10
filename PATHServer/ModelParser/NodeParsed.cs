using PATHServer.BDD.Models;
using PATHServer.ModelParser;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WebApplicationAPI
{
    public class NodeParsed
    {
        [Key]
        public int node_id { get; set; }

        [Required]
        [MaxLength(50)]
        public string node_name { get; set; }

        [Required]
        [DisplayName("node_type_data")]
        public int node_type_data { get; set; }

        public string InfoTypeData { get; set; }

        public NodeParsed(int node_id, string node_name, int node_type_data, string InfoTypeData)
        {
            this.node_id = node_id;
            this.node_name = node_name;
            this.node_type_data = node_type_data;
            this.InfoTypeData = InfoTypeData;
        }

        public static NodeParsed CreateFromModel(Node model)
        {
            return new NodeParsed(
                model.node_id,
                model.node_name,
                model.node_type_data,
                model.InfoTypeData.ToString()
                );
        }
    }
}
