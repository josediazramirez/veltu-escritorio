using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class AtencionDTO
    {
        public int idatencion { get; set; }
        public int tipo_atencion { get; set; }
        public int idtipo_atencion { get; set; }
        public string tip_atencion { get; set; }
        public string color_atencion { get; set; }
        public string vendedor { get; set; }
        public int num_atencion { get; set; }
        public int? idpedido { get; set; }
        public int? idventadevolucion { get; set; }
    }
}
