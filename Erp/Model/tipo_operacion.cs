namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tipo_operacion")]
    public partial class tipo_operacion
    {
        //idtipo_operacion, nombre, color
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tipo_operacion()
        {
        }

        [Key]
        public int idtipo_operacion { get; set; }

        public string nombre{ get; set; }

        public string color { get; set; }
    }
}
