using System.ComponentModel.DataAnnotations;

namespace BioAlga.Backend.Dtos
{
    public class ActualizarClienteDto
    {
        [MaxLength(100)]
        public string? Nombre { get; set; }

        [MaxLength(100)]
        public string? Apellido { get; set; }

        [MaxLength(100), EmailAddress]
        public string? Correo { get; set; }

        [MaxLength(20)]
        public string? Telefono { get; set; }

        public string? Direccion { get; set; }

        public string? Tipo_Cliente { get; set; }

        public string? Estado { get; set; }
    }
}
