namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Caja")]
    public partial class Caja
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Caja()
        {
        }

        [Key]
        public int codigo { get; set; }

        [Column(TypeName = "date")]
        public DateTime fecha { get; set; }

        public DateTime hora_inicio { get; set; }

        public DateTime? hora_termino { get; set; }

        public long? total { get; set; }

        [StringLength(10)]
        public string estado { get; set; }

        public int? total_descuento { get; set; }

        public int? efectivo_hay { get; set; }
        public int? efectivo_esperado { get; set; }
        public int? efectivo_diferencia { get; set; }
        public int? efectivo_inicio { get; set; }

        public int  idusuario { get; set; }
    }
}
