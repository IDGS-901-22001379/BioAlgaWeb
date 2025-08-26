namespace BioAlga.Backend.Dtos;

public class CajaQueryParams
{
    public int? IdUsuario { get; set; }
    public bool? Activa { get; set; }      // para filtrar aperturas
    public string? FechaDesde { get; set; }
    public string? FechaHasta { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } = "fecha_apertura";
    public string? SortDir { get; set; } = "desc";
}
