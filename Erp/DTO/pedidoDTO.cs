using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DTO
{
    public class pedidoDTO
    {
        public int codigo { get; set; }
        public string estado { get; set; }
        public string estado_color { get; set; }
        public DateTime fecha { get; set; }
        public string cl_nombre { get; set; }
        public int? cl_numero { get; set; }
        public string direccion { get; set; }
        public int? num_dire { get; set; }
        public int total { get; set; }
        public int numero { get; set; }
        public Visibility cancelar_visible
        {
            get
            {
                return estado== "CANCELADA" ? Visibility.Hidden:Visibility.Visible;
            }
        }
        public string Direccion
        {
            get
            {
                return direccion + " " + num_dire;
            }
        }
    }
}
