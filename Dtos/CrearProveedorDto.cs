using System.ComponentModel.DataAnnotations;

namespace BioAlga.Backend.Dtos
{
    public class CrearProveedorDto
    {
        [Required, MinLength(2), MaxLength(120)]
        public string Nombre_Empresa { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Contacto { get; set; }

        [EmailAddress, MaxLength(120)]
        public string? Correo { get; set; }

        [MaxLength(20)]
        public string? Telefono { get; set; }

        public string? Direccion { get; set; }

        [MaxLength(13)]
        public string? Rfc { get; set; }

        [MaxLength(50)]
        public string? Pais { get; set; }

        [MaxLength(50)]
        public string? Ciudad { get; set; }

        [MaxLength(10)]
        public string? Codigo_Postal { get; set; }
    }
}
