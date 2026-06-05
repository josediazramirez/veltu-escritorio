using System.ComponentModel.DataAnnotations;

namespace ErpApi.Domain.Entities;

public class Inventario
{
    [Key]
    public int IdInventario { get; set; }

    public int? IdIngrediente { get; set; }

    public int? IdProducto { get; set; }

    public int Stock_Minimo { get; set; }

    public int Stock_Total { get; set; }

    public int IdCentro { get; set; }

    public int IdMedida { get; set; }
}
