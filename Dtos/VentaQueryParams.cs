namespace BioAlga.Backend.Dtos
{
    public class VentaQueryParams
    {
        // Búsqueda libre (id de venta, nombre cliente, etc.)
        public string? Q { get; set; }

        // Filtros directos
        public int? ClienteId { get; set; }
        public int? UsuarioId { get; set; }
        public string? Estatus { get; set; }      // "Pagada" | "Cancelada"
        public string? MetodoPago { get; set; }   // "Efectivo" | "Tarjeta" | ...

        // Rango de fechas
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }

        // (Compatibilidad si ya mandabas strings desde el front)
        public string? FechaDesde { get; set; }
        public string? FechaHasta { get; set; }

        // Paginación y orden
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "fecha_venta";
        public string? SortDir { get; set; } = "desc";

        // Normaliza strings de fecha hacia Desde/Hasta si vienen
        public void NormalizeDates()
        {
            if (!Desde.HasValue && DateTime.TryParse(FechaDesde, out var d1))
                Desde = d1;
            if (!Hasta.HasValue && DateTime.TryParse(FechaHasta, out var d2))
                Hasta = d2;
        }
    }
}
