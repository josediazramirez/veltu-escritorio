namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("inventario")]
    public partial class inventario
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public inventario()
        {
        }

        [Key]
        public int idinventario { get; set; }
        public int? idingrediente { get; set; }
        public int? idproducto { get; set; }
        public int stock_minimo { get; set; }
        public int stock_total { get; set; }
        public int idcentro { get; set; }
        public int idmedida { get; set; }
    }
}
