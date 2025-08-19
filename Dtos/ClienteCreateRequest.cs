// Dtos/ClienteCreateRequest.cs
using System.ComponentModel.DataAnnotations;

namespace BioAlga.Backend.Dtos
{
    public class ClienteCreateRequest
    {
        [Required, MinLength(2), MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Apellido { get; set; }

        [EmailAddress, MaxLength(100)]
        public string? Correo { get; set; }

        [MaxLength(20)]
        public string? Telefono { get; set; }

        public string? Direccion { get; set; }

        // Opcionales; si no llegan, se aplican defaults del modelo/DB
        [MaxLength(20)]
        public string? Tipo_Cliente { get; set; } = "Normal";

        [MaxLength(10)]
        public string? Estado { get; set; } = "Activo";
    }
}
