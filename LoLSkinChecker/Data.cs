using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLSkinChecker
{
    public class Champion
    {
        public int id { get; set; }
        public string SearchID { get; set; }
        public string Name { get; set; }
        public List<Skin> Skins;

        public Champion(int Id, string[] Name, List<Skin> Skins)
        {
            this.id = Id;
            this.SearchID = Name[0];
            this.Name = Name[1];
            this.Skins = Skins;
        }
    }

    public class Skin
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Number { get; set; }
    }


}
