using System;

namespace BioAlga.Backend.Dtos
{
    public class DevolucionQueryParams
    {
        /// <summary>
        /// Fecha inicial para filtrar devoluciones.
        /// </summary>
        public DateTime? Desde { get; set; }

        /// <summary>
        /// Fecha final para filtrar devoluciones.
        /// </summary>
        public DateTime? Hasta { get; set; }

        /// <summary>
        /// Texto libre para búsqueda (motivo, referencia, usuario, etc.)
        /// </summary>
        public string? Q { get; set; }

        /// <summary>
        /// Si solo quieres devoluciones que regresan inventario.
        /// </summary>
        public bool? RegresaInventario { get; set; }
    }
}
