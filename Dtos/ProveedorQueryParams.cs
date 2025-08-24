namespace BioAlga.Backend.Dtos
{
    // Filtros + paginación + orden
    public class ProveedorQueryParams
    {
        public string? Q { get; set; }              // búsqueda libre
        public string? Estatus { get; set; }        // Activo/Inactivo
        public string? Pais { get; set; }
        public string? Ciudad { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string? SortBy { get; set; } = "Nombre_Empresa"; // campo dto
        public string? SortDir { get; set; } = "asc";           // asc/desc
    }
}
