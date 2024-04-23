using Model;
using System;
using System.Collections.Generic;

namespace DTO
{
    public class ComandaDTO
    {
        public int id { get; set; }
        public int numero { get; set; }
        public DateTime fecha { get; set; }
        public string  cliente { get; set; }
        public int? telefono { get; set; }
        public int? telefono_opc { get; set; }
        public string direccion { get; set; }
        public int? num_dire { get; set; }
        public string cod_pro { get; set; }
        public string producto { get; set; }
        public string cantidad { get; set; }
        public int? precio { get; set; }
        public string observacion { get; set; }
        public int total { get; set; }
        public string mediopago { get; set; }
        public string vendedor { get; set; }
        public string centro { get; set; }
        public int? flete { get; set; }
        public int? descuento { get; set; }
        public string Direccion
        {
            get
            {
                return direccion+" "+num_dire;
            }
        }
        public string despacho { get; set; }
        public List<ExtraIngrediente> ingrediente = new List<ExtraIngrediente>();
    }
}
