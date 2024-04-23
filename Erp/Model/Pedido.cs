namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Pedido")]
    public partial class Pedido
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Pedido()
        {
            Detalle_pedido = new HashSet<Detalle_pedido>();
        }

        [Key]
        public int codigo { get; set; }

        public int? numero { get; set; }

        public int? idcliente { get; set; }

        public DateTime fecha { get; set; }

        public int precio_total { get; set; }

        [StringLength(2000)]
        public string observacion { get; set; }

        public int? mp_id { get; set; }

        [StringLength(50)]
        public string cli_nombre { get; set; }

        public int? descuento { get; set; }

        
        [StringLength(200)]
        public string idComputador { get; set; }

        [StringLength(100)]
        public string despacho { get; set; }
        public int? idestado { get; set; }
        public int? idflete { get; set; }

        public virtual Cliente Cliente { get; set; }

        public virtual Computador Computador { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Detalle_pedido> Detalle_pedido { get; set; }

        public virtual MedioPago MedioPago { get; set; }
    }
}
