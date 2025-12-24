using System.Collections.Generic;
using DTO;

namespace ErpClass
{
    public class GlobalClass
    {
        //idpedido,nombre,pro_codigo,categoria,total,telefono,direccion,estado,observacion,cliente,tamanio,fecha
        public static int idpedido { get; set; }
        public static string nombre { get; set; }
        public static int pro_codigo { get; set; }
        public static int categoria { get; set; }
        public static int total { get; set; }
        public static int? telefono { get; set; }
        public static string direccion { get; set; }
        public static int estado { get; set; }
        public static string observacion { get; set; }
        public static string cliente { get; set; }
        public static int tamanio { get; set; }
        public static string fecha { get; set; }
        public static List<DetalleDTO> productos = new List<DetalleDTO>();
        public static List<AddIngDTO> ing = new List<AddIngDTO>();
        public static string IdComputador { get; set; }
        public static string NameComputador { get; set; }
        public static clienteDTO obj_cliente { get; set; }
        public static int num_pedido { get; set; }
        public static int? idcliente { get; set; }
        public static int user_rol { get; set; }
        public static string user_rol_nombre { get; set; }
        public static string user_nombre { get; set; }
        public static int   idusuario { get; set; }
        public static int? idcaja { get; set; }
        public static int? saldo_devolucion { get; set; }
        public static int? idventadevolucion { get; set; }
        public static FacturaDTO factura { get; set; }

    }
}
