using System.ComponentModel.DataAnnotations;

namespace BioAlga.Backend.Dtos
{
    public class CrearClienteDto
    {
        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Apellido { get; set; }

        [MaxLength(100), EmailAddress]
        public string? Correo { get; set; }

        [MaxLength(20)]
        public string? Telefono { get; set; }

        public string? Direccion { get; set; }

        [Required]
        public string Tipo_Cliente { get; set; } = "Normal";

        public string Estado { get; set; } = "Activo";
    }
}

