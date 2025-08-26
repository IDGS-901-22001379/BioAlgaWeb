using System.ComponentModel.DataAnnotations;

namespace BioAlga.Backend.Dtos;

public class VentaLineaCreate
{
    [Required] public int IdProducto { get; set; }

    [Range(1, int.MaxValue)]
    public int Cantidad { get; set; }

    [Range(0, 999999)]
    public decimal PrecioUnitario { get; set; }

    [Range(0, 999999)]
    public decimal DescuentoUnitario { get; set; } = 0m;

    [Range(0, 999999)]
    public decimal IvaUnitario { get; set; } = 0m;
}
