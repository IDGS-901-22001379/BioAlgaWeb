// Dtos/ClienteQueryParams.cs
namespace BioAlga.Backend.Dtos
{
    public class ClienteQueryParams
    {
        public string? Q { get; set; }                   // búsqueda libre por nombre, apellido, correo, teléfono
        public string? Estado { get; set; }              // Activo | Inactivo
        public string? Tipo_Cliente { get; set; }        // Normal | Mayorista | Premium
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Filtro por rango de fechas (UTC)
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
    }
}
