namespace BioAlga.Backend.Dtos
{
    public class DevolucionQueryParams
    {
        public string? Q { get; set; }                   // busca en motivo, referencia, usuario_nombre
        public int? IdProducto { get; set; }             // filtra por producto
        public int? IdUsuario { get; set; }              // filtra por usuario que registró
        public bool? RegresaInventario { get; set; }     // 1/0
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }

        // Paginación simple:
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Orden:
        // fecha_desc (default), fecha_asc, total_desc, total_asc
        public string? SortBy { get; set; } = "fecha_desc";
    }
}
