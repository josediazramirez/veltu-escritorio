using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DTO
{
    public class FacturaDTO
    {

        public int idfactura { get; set; }

        public string rut { get; set; }

        public string correo { get; set; }
        public int numero { get; set; }
        public int idpedido { get; set; }
        public DateTime fecha { get; set; }
        public int estado { get; set; }
        public string fecha_texto
        {
            get
            {
                return fecha.ToString("dd-MM-yyyy");
            }
        }
        public string estado_color { 
            get
            {
                return estado == 0 ? "red" : "green";
            }
        }
        public string estado_texto { 
            get
            {
                return estado == 0 ? "PENDIENTE" : "HECHA";
            }
        }
    }
}
