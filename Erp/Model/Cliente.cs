namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Cliente")]
    public partial class Cliente
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Cliente()
        {
            Pedido = new HashSet<Pedido>();
        }
        [Key]
        public int idcliente { get; set; }

        public int id_telefono { get; set; }

        
        [StringLength(50)]
        public string nombre { get; set; }

        
        [StringLength(200)]
        public string direccion { get; set; }

        public int? num_direccion { get; set; }

        public int? telefono_opc { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Pedido> Pedido { get; set; }
    }
}
