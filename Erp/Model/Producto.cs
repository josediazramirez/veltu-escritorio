namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Producto")]
    public partial class Producto
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Producto()
        {
        }

        [Key]
        public int codigo { get; set; }

        [Required]
        [StringLength(200)]
        public string nombre { get; set; }

        public int? categoria_codigo { get; set; }

        public int? precio { get; set; }
        public int? precio_costo { get; set; }
        public int? estado { get; set; }
        public int? idmarca { get; set; }
        public int? idcolor { get; set; }
        public string ean_codigo { get; set; }
        public string descripcion { get; set; }
        public string lote { get; set; }
        public string imagen1 { get; set; }

    }

}