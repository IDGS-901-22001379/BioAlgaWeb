namespace BioAlga.Backend.Dtos;

public class DevolucionQueryParams
{
    public int? IdVenta { get; set; }
    public string? FechaDesde { get; set; }
    public string? FechaHasta { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } = "fecha";
    public string? SortDir { get; set; } = "desc";
}
