using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BioAlga.Backend.Dtos
{
    public class DevolucionCreateRequest
    {
        /// <summary>
        /// Id de la venta a la que está ligada la devolución (opcional).
        /// </summary>
        public int? VentaId { get; set; }

        [MaxLength(50)]
        public string? ReferenciaVenta { get; set; }

        [Required, MaxLength(120)]
        public string UsuarioNombre { get; set; } = string.Empty;

        [Required, MaxLength(300)]
        public string Motivo { get; set; } = string.Empty;

        public bool RegresaInventario { get; set; } = true;

        public string? Notas { get; set; }

        [Required, MinLength(1)]
        public List<DevolucionLineaCreate> Lineas { get; set; } = new();
    }
}
