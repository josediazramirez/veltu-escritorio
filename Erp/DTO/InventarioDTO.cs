using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class InventarioDTO
    {
        public int cod_inv { get; set; }
        public int cod_pro { get; set; }
        public string nombre { get; set; }
        public string categoria { get; set; }
        public int idcategoria { get; set; }

        public string uni_medida { get; set; }
        public int iduni_medida { get; set; }
        public string ubicacion { get; set; }
        public int idubicacion { get; set; }

        public string stock_minimo { get; set; }
        public string stock_total { get; set; }

        public string stock_minimo_color { get; set; }
        public string ean { get; set; }
    }
}
