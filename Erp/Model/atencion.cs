namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("atencion")]
    public partial class atencion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public atencion()
        {
        }

        [Key]
        public int idatencion { get; set; }
        public int idestado_atencion { get; set; }
        public int numero_atencion { get; set; }
        public int idtipoatencion { get; set; }
        public DateTime fecha { get; set; }
     
        public int? idpedido { get; set; }
        public int? idventadevolucion { get; set; }

        public string vendedor { get; set; }
    }
}
