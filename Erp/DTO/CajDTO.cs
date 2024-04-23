using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DTO
{
    public class CajDTO
    { 
        
        public int codigo { get; set; }
        public string estado { get; set; }
        public DateTime fecha { get; set; }
        public string hora_inicio { get; set; }
        public string hora_termino { get; set; }
        public long? total { get; set; }


    }
}
