using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class MovimientoDTO
    {
        public int id { get; set; }
        public DateTime fecha_hora { get; set; }
        public int cod_pro { get; set; }
        public string nombre_pro { get; set; }
        public string movimiento { get; set; }
        public string cantidad { get; set; }
        public string ubicacion { get; set; }
        public string stock_ant { get; set; }
        public string stock_act { get; set; }
        public string mov_color { get; set; }
        public string destino { get; set; }
    }
}
