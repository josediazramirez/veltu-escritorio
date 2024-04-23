namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("centro")]
    public partial class centro
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public centro()
        {
        }

        [Key]
        public int idcentro { get; set; }

        [Required]
        public string nombre { get; set; }
        public int estado { get; set; }
        public int cansell { get; set; }
        public int autorizacion { get; set; }
    }
}
