// Dtos/ClienteUpdateRequest.cs
using System.ComponentModel.DataAnnotations;

namespace BioAlga.Backend.Dtos
{
    public class ClienteUpdateRequest
    {
        [MinLength(2), MaxLength(100)]
        public string? Nombre { get; set; }

        [MaxLength(100)]
        public string? Apellido { get; set; }

        [EmailAddress, MaxLength(100)]
        public string? Correo { get; set; }

        [MaxLength(20)]
        public string? Telefono { get; set; }

        public string? Direccion { get; set; }

        [MaxLength(20)]
        public string? Tipo_Cliente { get; set; } // Normal | Mayorista | Premium

        [MaxLength(10)]
        public string? Estado { get; set; } // Activo | Inactivo
    }
}
