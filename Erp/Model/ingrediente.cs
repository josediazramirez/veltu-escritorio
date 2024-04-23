namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ingrediente")]
    public partial class ingrediente
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ingrediente()
        {
        }

        [Key]
        public int idingre { get; set; }

        [Required]
        [StringLength(45)]
        public string nombre { get; set; }
    }
}
