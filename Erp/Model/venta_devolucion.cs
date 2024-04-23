namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("venta_devolucion")]
    public partial class venta_devolucion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public venta_devolucion()
        {
        }

        [Key]
        public int idventadevolucion { get; set; }
        public int total_devolucion { get; set; }
        public int idestadodevolucion { get; set; }
        public int? idpedido { get; set; }
        
        public string motivo { get; set; }
    }
}
