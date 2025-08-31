using System;
using System.Collections.Generic;

namespace BioAlga.Backend.Dtos
{
    public class DevolucionDto
    {
        public int IdDevolucion { get; set; }
        public DateTime FechaDevolucion { get; set; }

        public int IdUsuario { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;

        public string Motivo { get; set; } = string.Empty;
        public bool RegresaInventario { get; set; }

        public decimal TotalDevuelto { get; set; }

        public int? VentaId { get; set; }
        public string? ReferenciaVenta { get; set; }
        public string? Notas { get; set; }

        public List<DevolucionDetalleDto> Detalles { get; set; } = new();
    }
}
