namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("pro_agre")]
    public partial class pro_agre
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public pro_agre()
        {
        }

        [Key,Column(Order = 0)]
        public int idproducto { get; set; }

        [Key, Column(Order = 1)]
        public int idagregado { get; set; }

    }
}
