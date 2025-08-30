using System.ComponentModel.DataAnnotations;

namespace BioAlga.Backend.Dtos
{
    /// <summary>
    /// Request para crear una Devolución SIN obligar número de venta.
    /// El backend calculará TotalDevuelto = suma de líneas.
    /// Además, sobreescribirá UsuarioNombre usando el usuario autenticado.
    /// </summary>
    public class DevolucionCreateRequest
    {
        [Required]
        [MaxLength(300)]
        public string Motivo { get; set; } = string.Empty;

        /// <summary>
        /// 1 = regresa inventario; 0 = no (dañado).
        /// </summary>
        public bool RegresaInventario { get; set; } = true;

        /// <summary>
        /// Opcional: ticket/folio si se conoce.
        /// </summary>
        [MaxLength(50)]
        public string? ReferenciaVenta { get; set; }

        /// <summary>
        /// Observaciones opcionales.
        /// </summary>
        public string? Notas { get; set; }

        /// <summary>
        /// Líneas devueltas. Debe tener al menos 1.
        /// </summary>
        [MinLength(1)]
        public List<DevolucionLineaCreate> Lineas { get; set; } = new();
    }
}
