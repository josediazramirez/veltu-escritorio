namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ExtraIngrediente")]
    public partial class ExtraIngrediente
    {
        [Key]
        public int ex_id { get; set; }

        public string  nombre { get; set; }

        public int ex_precio { get; set; }

        public int? id_detalle { get; set; }
        public int? id_ingre { get; set; }
        public int? id_agregado { get; set; }

        public virtual Detalle_pedido Detalle_pedido { get; set; }
    }
}
