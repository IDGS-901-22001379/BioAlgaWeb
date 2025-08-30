using System;

namespace BioAlga.Backend.Dtos
{
    /// <summary>
    /// DTO para listar y ver detalle de una devolución.
    /// </summary>
    public class DevolucionDto
    {
        public int IdDevolucion { get; set; }
        public DateTime FechaDevolucion { get; set; }

        public int IdUsuario { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;

        public string Motivo { get; set; } = string.Empty;
        public bool RegresaInventario { get; set; }

        /// <summary>
        /// Total devuelto (lo que se descuenta de ventas del día).
        /// </summary>
        public decimal TotalDevuelto { get; set; }

        public string? ReferenciaVenta { get; set; }
        public string? Notas { get; set; }

        public int NumeroLineas { get; set; }         // para listados rápidos
        public List<DevolucionDetalleDto> Detalles { get; set; } = new();
    }
}
