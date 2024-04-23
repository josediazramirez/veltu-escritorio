namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("medida")]
    public partial class medida
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public medida()
        {
        }

        [Key]
        public int idmedida { get; set; }

        [Required]
        [StringLength(45)]
        public string sigla { get; set; }
        [Required]
        [StringLength(45)]
        public string nombre { get; set; }
    }
}
