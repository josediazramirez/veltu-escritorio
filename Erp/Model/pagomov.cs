namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("pagomov")]
    public partial class pagomov
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public pagomov()
        {
        }

        [Key]
        public int idpagomov { get; set; }
        public int mp_id{ get; set; }
        public int total { get; set; }
        public int vuelto { get; set; }
        public int descuento { get; set; }
        public int? idmovcaja { get; set; }
        
    }
}
