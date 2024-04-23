using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class CategoriaTypeDTO
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public int estado { get; set; }
        public string NameEstado { 
            get
            {
                return estado==1?"Activo":"";
            }
        }
    }
}
