using System.ComponentModel.DataAnnotations;

namespace BioAlga.Backend.Dtos
{
    public class CrearClienteDto
    {
        [Required, MinLength(2), MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Apellido_Paterno { get; set; }

        [MaxLength(100)]
        public string? Apellido_Materno { get; set; }

        [EmailAddress, MaxLength(100)]
        public string? Correo { get; set; }

        [MaxLength(20)]
        public string? Telefono { get; set; }

        public string? Direccion { get; set; }

        // Validaciones suaves (se validará fuerte en Service/DB)
        [MaxLength(20)]
        public string Tipo_Cliente { get; set; } = "Normal"; // Normal, Mayoreo, Especial, Descuento

        [MaxLength(20)]
        public string Estado { get; set; } = "Activo"; // Activo, Inactivo
    }
}
