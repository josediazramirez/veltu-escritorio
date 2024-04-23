namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("factura")]
    public partial class factura
    {
        //idtipo_operacion, nombre, color
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public factura()
        {
        }
        //idfactura, rut, correo, numero, idpedido, fecha, estado
        [Key]
        public int idfactura { get; set; }

        public string rut { get; set; }

        public string correo { get; set; }
        public int numero { get; set; }
        public int idpedido { get; set; }
        public DateTime fecha { get; set; }
        public int estado { get; set; }
    }
}
