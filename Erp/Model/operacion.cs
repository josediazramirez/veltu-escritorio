namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("operacion")]
    public partial class operacion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public operacion()
        {
        }

        [Key]
        public int idoperacion { get; set; }
        public int idtipo_operacion { get; set; }
        public int idinventario { get; set; }
        public DateTime fecha_hora { get; set; }
        public int cantidad { get; set; }
        public int? idproducto { get; set; }
        public int? idingrediente { get; set; }
        public int stock_ant { get; set; }
        public int stock_act { get; set; }
        public int? idtrans_bodega { get; set; }
    }
}
