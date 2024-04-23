using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class UsuarioDTO
    {
        public int idusuario { get; set; }
        public string login { get; set; }

        public string nombre { get; set; }
        public string ape_pa { get; set; }
        public string ape_ma { get; set; }
        public string correo { get; set; }
        public int habilitado { get; set; }
        public int idrol { get; set; }
        public string apellidos
        {
            get
            {
                return ape_pa + " " + ape_ma;
            }
        }
        public string    roles { get; set; }
        public string estado
        {
            get
            {
                if (habilitado == 1)
                {
                    return "ACTIVO";
                }
                return "";
            }
        }
    }
}
