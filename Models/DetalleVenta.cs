using System;

namespace BioAlga.Backend.Models;

public class DetalleVenta
{
    public int IdDetalle { get; set; }
    public int IdVenta { get; set; }        // FK ventas.id_venta
    public int IdProducto { get; set; }     // FK productos.id_producto

    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal DescuentoUnitario { get; set; } = 0m;
    public decimal IvaUnitario { get; set; } = 0m;

    // Navs
    public Venta Venta { get; set; } = default!;
    public Producto Producto { get; set; } = default!;
}
