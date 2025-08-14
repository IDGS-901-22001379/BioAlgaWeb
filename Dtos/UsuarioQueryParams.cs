namespace BioAlga.Backend.Dtos
{
    public class UsuarioQueryParams
    {
        public string? Nombre { get; set; }
        public bool? Activo { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
