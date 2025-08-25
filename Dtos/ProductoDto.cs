using BioAlga.Backend.Models.Enums;

namespace BioAlga.Backend.Dtos
{
    public class ProductoDto
    {
        public int Id_Producto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public TipoProducto Tipo { get; set; }
        public int? Id_Categoria { get; set; }
        public int? Id_Marca { get; set; }
        public int? Id_Unidad { get; set; }
        public int? Proveedor_Preferente_Id { get; set; }
        public string Codigo_Sku { get; set; } = string.Empty;
        public string? Codigo_Barras { get; set; }
        public string Estatus { get; set; } = "Activo";
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }

        // Opcional: precios vigentes para pintar tabla (si quieres traerlos directo)
        public decimal? PrecioNormal { get; set; }
        public decimal? PrecioMayoreo { get; set; }
        public decimal? PrecioDescuento { get; set; }
        public decimal? PrecioEspecial { get; set; }
    }
}
