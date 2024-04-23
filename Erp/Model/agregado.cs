namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("agregado")]
    public partial class agregado
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public agregado()
        {
        }

        [Key]
        public int idagregado { get; set; }

        [Required]
        public int idingre { get; set; }
        [Required]
        public int idcategoria { get; set; }
        [Required]
        public int precio { get; set; }
        public int cantidad { get; set; }
        public int idmedida { get; set; }

    }
}
