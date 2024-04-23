namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("rol")]
    public partial class rol
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public rol()
        {
        }

        [Key]
        public int idrol { get; set; }

        [Required]
        [StringLength(45)]
        public string nombre{ get; set; }
        [StringLength(45)]
        public string descripcion { get; set; }
    }
}
