using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class AgregadoDTO
    {
        public int cod_agregado { get; set; }
        public int cod_ingrediente { get; set; }
        public string nombre { get; set; }
        public string categoria { get; set; }
        public int precio { get; set; }
        public string uni_medida { get; set; }
        public string porcion_text 
        { 
            get { return porcion+" "+(uni_medida==null?"":uni_medida.ToUpper()); }
            set { } 
        }
        public int porcion { get; set; }
        public int categoria_id { get; set; }
        public int uni_medida_id { get; set; }
        
    }
}
