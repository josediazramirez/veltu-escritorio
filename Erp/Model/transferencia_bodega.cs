namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("transferencia_bodega")]
    public partial class transferencia_bodega
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public transferencia_bodega()
        {
        }

        [Key]
        public int idtrans_bodega { get; set; }

        public int idcentrodesde { get; set; }
        public int idcentrohasta { get; set; }
        public string descripcion { get; set; }
        public int login_user_revisa { get; set; }
        public DateTime fecharevision { get; set; }
        public int login_user_autoriza { get; set; }
        public DateTime fechaautorizacion { get; set; }
    }
}
