using PATHServer.BDD.Models;
using PATHServer.ModelParser;

namespace WebApplicationAPI
{
    public class PathUserParsed
    {
        public int pu_id { get; private set; }
        public string pu_name { get; private set; }
        public string pu_surname { get; private set; }

        public PathUserParsed(int pu_id, string pu_name, string pu_surname)
        {
            this.pu_id = pu_id;
            this.pu_name = pu_name;
            this.pu_surname = pu_surname;
        }

        public static PathUserParsed CreateFromModel(PATHUser model)
        {
            return new PathUserParsed(
                model.pu_id,
                model.pu_name,
                model.pu_surname
                );
        }
    }
}
