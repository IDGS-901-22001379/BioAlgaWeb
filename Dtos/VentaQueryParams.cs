namespace BioAlga.Backend.Dtos;

public class VentaQueryParams
{
    // Filtros básicos para listado/búsqueda de ventas
    public string? Q { get; set; }              // folio, cliente, usuario (lo que decidas mapear)
    public string? FechaDesde { get; set; }     // ISO date
    public string? FechaHasta { get; set; }     // ISO date
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } = "fecha_venta";
    public string? SortDir { get; set; } = "desc";
}
