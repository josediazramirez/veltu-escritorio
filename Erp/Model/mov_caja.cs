namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("mov_caja")]
    public partial class mov_caja
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public mov_caja()
        {
        }

        [Key]
        public int idmovcaja { get; set; }
        public int idtipomov{ get; set; }
        public int total_ent { get; set; }
        public int total_sal { get; set; }
        public int idcaja { get; set; }
        public string observacion { get; set; }
        public int? idpedido { get; set; }
        public int? idventadevolucion { get; set; }
        public DateTime? mov_fecha { get; set; }
    }
}
