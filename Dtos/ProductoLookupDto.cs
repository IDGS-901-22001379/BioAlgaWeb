namespace BioAlga.Backend.Dtos;

public class ProductoLookupDto
{
    public int Id_Producto { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string? Codigo_Barras { get; set; }
    public string Estatus { get; set; } = "Activo";
}
