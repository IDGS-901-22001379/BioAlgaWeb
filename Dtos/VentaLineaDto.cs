// Dtos/VentaLineaDto.cs
namespace BioAlga.Backend.Dtos
{
    /// Detalle (línea) de la venta
    public class VentaLineaDto
    {
        public int IdDetalle { get; set; }

        public int IdProducto { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public string? CodigoSku { get; set; }

        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }
        public decimal DescuentoUnitario { get; set; }
        public decimal IvaUnitario { get; set; }

        // Importes calculados para mostrar en el front
        public decimal ImporteBruto     => PrecioUnitario * Cantidad;
        public decimal ImporteDescuento => DescuentoUnitario * Cantidad;
        public decimal ImporteIva       => IvaUnitario * Cantidad;
        public decimal ImporteNeto      => ImporteBruto - ImporteDescuento + ImporteIva;
    }
}
