namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Detalle_pedido")]
    public partial class Detalle_pedido
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Detalle_pedido()
        {
            ExtraIngrediente = new HashSet<ExtraIngrediente>();
        }

        [Key]
        public int id_detalle { get; set; }

        public int cantidad { get; set; }

        public int precio { get; set; }

        public int codigo { get; set; }

        public int pedido { get; set; }

        public int? tamanio { get; set; }
        public int? idinventario { get; set; }
        public int? precio_desc { get; set; }
        public int total { get; set; }
        

[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExtraIngrediente> ExtraIngrediente { get; set; }

        public virtual Pedido Pedido1 { get; set; }

        public virtual Producto Producto { get; set; }
    }
}
