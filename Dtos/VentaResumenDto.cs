// Dtos/VentaResumenDto.cs
using BioAlga.Backend.Models.Enums;

namespace BioAlga.Backend.Dtos
{
    /// Resumen para listar ventas (historial)
    public class VentaResumenDto
    {
        public int IdVenta { get; set; }
        public DateTime FechaVenta { get; set; }

        public int? ClienteId { get; set; }
        public string? ClienteNombre { get; set; }  // “Nombre ApellidoP ApellidoM”

        public decimal Subtotal { get; set; }
        public decimal Impuestos { get; set; }
        public decimal Total { get; set; }

        public MetodoPago MetodoPago { get; set; }
        public EstatusVenta Estatus { get; set; }

        public int Partidas { get; set; }  // líneas de la venta
        public int Unidades { get; set; }  // suma de cantidades

        public int IdUsuario { get; set; }
        public string? UsuarioNombre { get; set; }
    }
}
