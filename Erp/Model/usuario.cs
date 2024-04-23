namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("usuario")]
    public partial class usuario
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public usuario()
        {
        }

        [Key]
        public int idusuario { get; set; }

        
        [StringLength(45)]
        public string login_usuario{ get; set; }

        [StringLength(45)]
        public string nombre { get; set; }
        [StringLength(45)]
        public string ape_paterno { get; set; }
        [StringLength(45)]
        public string ape_materno { get; set; }
        [StringLength(45)]
        public string correo { get; set; }
        [StringLength(45)]
        public string password { get; set; }

        public int idrol { get; set; }
        public int habilitado { get; set; }
        public string key { get; set; }
    }
}
