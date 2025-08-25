namespace BioAlga.Backend.Dtos;

public class CompraDto
{
    public int Id_Compra { get; set; }
    public int? Proveedor_Id { get; set; }
    public string? Proveedor_Texto { get; set; }
    public DateTime Fecha_Compra { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Impuestos { get; set; }
    public decimal Total { get; set; }
    public int Id_Usuario { get; set; }
    public string? Notas { get; set; }
    public string Estado { get; set; } = "Borrador";

    public List<DetalleCompraDto> Detalles { get; set; } = new();
}

public class DetalleCompraDto
{
    public int Id_Detalle { get; set; }
    public int Id_Producto { get; set; }
    public string? Producto { get; set; }
    public string? SKU { get; set; }
    public int Cantidad { get; set; }
    public decimal Costo_Unitario { get; set; }
    public decimal Iva_Unitario { get; set; }
}
