namespace Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("factura")]
    public partial class factura
    {
        [Key]
        public int idfactura { get; set; }

        [Column(TypeName = "varchar")]
        [MaxLength(45)]
        public string rut { get; set; }

        [Column(TypeName = "varchar")]
        [MaxLength(45)]
        public string correo { get; set; }

        public int numero { get; set; }
        public int idpedido { get; set; }
        public DateTime fecha { get; set; }
        public int estado { get; set; }
    }
}