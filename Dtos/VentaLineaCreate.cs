// Dtos/VentaLineaCreate.cs
using System.ComponentModel.DataAnnotations;

namespace BioAlga.Backend.Dtos
{
    /// <summary>Línea (detalle) para crear una venta.</summary>
    public class VentaLineaCreate
    {
        /// <summary>Id del producto (FK productos.id_producto).</summary>
        [Required]
        public int IdProducto { get; set; }

        /// <summary>Cantidad de unidades.</summary>
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }

        /// <summary>Precio unitario aplicado (ya resuelto según tipo de cliente).</summary>
        [Range(0, 999999, ErrorMessage = "El precio unitario no puede ser negativo.")]
        public decimal PrecioUnitario { get; set; }

        /// <summary>Descuento unitario aplicado (si no hay, 0).</summary>
        [Range(0, 999999, ErrorMessage = "El descuento unitario no puede ser negativo.")]
        public decimal DescuentoUnitario { get; set; } = 0m;

        /// <summary>IVA unitario calculado para la línea.</summary>
        [Range(0, 999999, ErrorMessage = "El IVA unitario no puede ser negativo.")]
        public decimal IvaUnitario { get; set; } = 0m;
    }
}
