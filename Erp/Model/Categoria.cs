namespace Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("Categoria")]
    public partial class Categoria
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Categoria()
        {
        }

        [Key]
        public int codigo { get; set; }

        [Required]
        [StringLength(10)]
        public string nombre { get; set; }
        public int estado { get; set; }

    }
}
