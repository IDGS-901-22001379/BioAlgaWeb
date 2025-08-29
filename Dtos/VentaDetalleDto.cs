// Dtos/VentaDetalleDto.cs
using BioAlga.Backend.Models.Enums;

namespace BioAlga.Backend.Dtos
{
    /// Encabezado + colección de líneas (para ver “ver detalles”)
    public class VentaDetalleDto
    {
        // Encabezado
        public int IdVenta { get; set; }
        public DateTime FechaVenta { get; set; }

        public int? ClienteId { get; set; }
        public string? ClienteNombre { get; set; }

        public decimal Subtotal { get; set; }
        public decimal Impuestos { get; set; }
        public decimal Total { get; set; }
        public decimal? EfectivoRecibido { get; set; }
        public decimal? Cambio { get; set; }

        public MetodoPago MetodoPago { get; set; }
        public EstatusVenta Estatus { get; set; }

        public int IdUsuario { get; set; }
        public string? UsuarioNombre { get; set; }

        // Agregados útiles
        public int Partidas { get; set; }
        public int Unidades { get; set; }

        // Detalles
        public List<VentaLineaDto> Detalles { get; set; } = new();
    }
}
