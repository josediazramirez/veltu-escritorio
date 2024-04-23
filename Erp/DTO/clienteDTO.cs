using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class clienteDTO
    {
        public int idcliente { get; set; }
        public int id_telefono { get; set; }
        public string nombre { get; set; }
        public string direccion { get; set; }
        public int? num_direccion { get; set; }
        public int? telefono_opc { get; set; }
    }
}
