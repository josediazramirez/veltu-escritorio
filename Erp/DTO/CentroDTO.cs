using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class CentroDTO
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public int estado { get; set; }
        public int CanSell { get; set; }
        public int autorizacion { get; set; }
        public string NameEstado
        {
            get
            {
                return estado == 1 ? "Activo":"";
            }
        }
        public string PuedeVender { 
            get {
                return CanSell==1?"Habilitado":"";
            }
        }
        public string GetAutorizacion
        {
            get
            {
                return autorizacion == 1 ? "Habilitado" : "";
            }
        }
        public System.Windows.Visibility CanSellVisibilidad { get { return this.id == 1 ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible; } set { this.CanSellVisibilidad = value; } }

    }
}
