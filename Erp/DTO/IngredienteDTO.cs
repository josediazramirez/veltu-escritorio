using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class IngredienteDTO 
    {  
        public int id { get; set; }
        public string nombre { get; set; }
        public string tamanio { get; set; }
        public int? precio { get; set; }
        public string tipo { get; set; }
        public bool Estado { get; set; }
        public int categoria { get; set; }
    }
}
