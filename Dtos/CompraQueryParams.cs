namespace BioAlga.Backend.Dtos;

public class CompraQueryParams
{
    public string? Q { get; set; }          // folio (id), proveedor texto o nombre
    public DateTime? Desde { get; set; }
    public DateTime? Hasta { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
