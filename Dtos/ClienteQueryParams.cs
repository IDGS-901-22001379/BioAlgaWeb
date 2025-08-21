using System.ComponentModel.DataAnnotations;

namespace BioAlga.Backend.Dtos
{
    public class ClienteQueryParams
    {
        // Texto libre para buscar por nombre y apellidos (y opcionalmente correo/teléfono)
        public string? Q { get; set; }

        // Filtros
        public string? Tipo_Cliente { get; set; } // Normal | Mayoreo | Especial | Descuento
        public string? Estado { get; set; }       // Activo | Inactivo

        // Paginación
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [Range(1, 200)]
        public int PageSize { get; set; } = 10;

        // Ordenamiento simple (opcional): nombre ASC/DESC
        public string? SortBy { get; set; } = "nombre"; // nombre | fecha
        public string? SortDir { get; set; } = "asc";    // asc | desc
    }
}
