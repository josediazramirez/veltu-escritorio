using System.ComponentModel.DataAnnotations;

namespace ErpApi.Domain.Entities;

public class Categoria
{
    [Key]
    public int Codigo { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = null!;

    public int Estado { get; set; }
}
