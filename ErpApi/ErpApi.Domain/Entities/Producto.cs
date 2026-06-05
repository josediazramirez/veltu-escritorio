using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErpApi.Domain.Entities;

public class Producto
{
    [Key]
    public int Codigo { get; set; }

    [Required]
    [MaxLength(200)]
    public string Nombre { get; set; } = null!;

    public int? Categoria_Codigo { get; set; }

    public int? Precio { get; set; }

    public int? Precio_Costo { get; set; }

    public int? Estado { get; set; }

    public int? IdMarca { get; set; }

    public int? IdColor { get; set; }

    [MaxLength(50)]
    public string? Ean_Codigo { get; set; }

    public string? Descripcion { get; set; }

    [MaxLength(50)]
    public string? Lote { get; set; }

    public string? Imagen1 { get; set; }
}
