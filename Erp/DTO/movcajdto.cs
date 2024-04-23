using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DTO
{
    public class MovCajaDTO
    {
        public int idmovcaja { get; set; }
        public DateTime mov_fecha { get; set; }
        public string tipomov { get; set; }

        public string descripcion { get; set; }
        public int total { get; set; }
    }
}
