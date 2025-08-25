namespace BioAlga.Backend.Dtos
{
    public class ProductoQueryParams
    {
        public string? Q { get; set; }                 // nombre/SKU/código de barras :contentReference[oaicite:10]{index=10}
        public string? Tipo { get; set; }              // Componente/Sensor/…
        public int? IdCategoria { get; set; }
        public int? IdMarca { get; set; }
        public int? IdUnidad { get; set; }
        public string? Estatus { get; set; }           // Activo/Inactivo
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }            // nombre, sku, created_at…
        public string? SortDir { get; set; }           // asc/desc
    }
}
