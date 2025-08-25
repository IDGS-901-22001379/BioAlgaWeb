namespace BioAlga.Backend.Dtos;

public class CrearCompraDto
{
    public int? Proveedor_Id { get; set; }
    public string? Proveedor_Texto { get; set; }
    public string? Notas { get; set; }
    public int Id_Usuario { get; set; }
}

public class AgregarRenglonDto
{
    public int Id_Producto { get; set; }
    public int Cantidad { get; set; }
    public decimal Costo_Unitario { get; set; }
    public decimal Iva_Unitario { get; set; } = 0m;
}

public class ConfirmarCompraResponse
{
    public int Id_Compra { get; set; }
    public int MovimientosCreados { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Impuestos { get; set; }
    public decimal Total { get; set; }
}
