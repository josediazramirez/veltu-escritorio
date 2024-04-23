namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MedioPago")]
    public partial class MedioPago
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MedioPago()
        {
            Pedido = new HashSet<Pedido>();
        }

        [Key]
        public int mp_id { get; set; }

        [Required]
        [StringLength(30)]
        public string mp_nombre { get; set; }

        [Required]
        [StringLength(100)]
        public string mp_desc { get; set; }
        public int? idpedido { get; set; }
        public int? total { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Pedido> Pedido { get; set; }
    }
}
