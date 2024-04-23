using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DTO
{
    public class CajaDTO
    {
        //codigo, fecha, hora_inicio, hora_termino, total, estado
        public int codigo { get; set; }
        public string estado { get; set; }
        public long? total { get; set; }
        public long? totaldescuento { get; set; }
        public DateTime fecha { get; set; }
        public DateTime hora_inicio { get; set; }
        public DateTime? hora_termino { get; set; }
        public int vista { get; set; }
        public Visibility visibility { get; set; }
        public string user_nombre { get; set; }
        public string user_apellido { get; set; }
        public int  idusuario { get; set; }

        public int? saldo_inicial { get; set; }
        public int? saldo_diferencia { get; set; }
        public int? saldo_actual { get; set; }
        public int? saldo_cierre { get; set; }


    }
}
