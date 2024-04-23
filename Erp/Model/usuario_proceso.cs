namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("usuario_proceso")]
    public partial class usuario_proceso
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public usuario_proceso()
        {
        }
        [Key, Column(Order = 0)]
        public int idusuario { get; set; }
        [Key, Column(Order = 1)]
        public int proce_id { get; set; }
    }
}
