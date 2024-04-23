namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("autorizacion_proceso")]
    public partial class autorizacion_proceso
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public autorizacion_proceso()
        {
        }
        //auto_id, auto_fecha, auto_mensaje, auto_estado, proce_id, idusuario
        [Key]
        public int auto_id { get; set; }
        public DateTime auto_fecha { get; set; }
        public string auto_mensaje { get; set; }
        public string auto_estado { get; set; }
        public int proce_id { get; set; }
        public int? idusuario { get; set; }
    }
}
