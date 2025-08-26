namespace BioAlga.Backend.Models;

public class DetalleDevolucion
{
    public int IdDetalle { get; set; }
    public int IdDevolucion { get; set; }   // FK devoluciones.id_devolucion
    public int IdProducto { get; set; }     // FK productos.id_producto

    public int Cantidad { get; set; }       // devuelta (≤ vendida)
    public decimal PrecioUnitario { get; set; }
    public decimal IvaUnitario { get; set; }

    // Navs
    public Devolucion Devolucion { get; set; } = default!;
    public Producto Producto { get; set; } = default!;
}
