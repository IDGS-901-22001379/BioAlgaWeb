using System.ComponentModel.DataAnnotations;

namespace BioAlga.Backend.Dtos
{
    public class DevolucionLineaCreate
    {
        [Required]
        public int IdProducto { get; set; }

        [Required, MaxLength(150)]
        public string ProductoNombre { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
        public int Cantidad { get; set; }

        /// <summary>
        /// Si está ligada a un detalle de venta.
        /// </summary>
        public int? IdDetalleVenta { get; set; }

        /// <summary>
        /// Precio unitario (solo obligatorio si no hay IdDetalleVenta).
        /// </summary>
        public decimal? PrecioUnitario { get; set; }
    }
}
