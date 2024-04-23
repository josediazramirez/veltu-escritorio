using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class AddIngDTO
    {
        public int id_agre { get; set; }
        public int id_ingre { get; set; }
        public string nombre { get; set; }
        public int precio { get; set; }
        public bool estadoo { get; set; }
        public string tipo { get; set; }
        public int id_tamanio { get; set; }
        public int porcion { get; set; }

    }
}
