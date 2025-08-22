using System;
using System.ComponentModel.DataAnnotations;

namespace BioAlga.Backend.Dtos
{
    /// <summary>
    /// DTO para crear empleado.
    /// </summary>
    public class CrearEmpleadoDto
    {
        [Required, MinLength(2), MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Apellido_Paterno { get; set; }

        [MaxLength(100)]
        public string? Apellido_Materno { get; set; }

        [MaxLength(18)]
        public string? Curp { get; set; }

        [MaxLength(13)]
        public string? Rfc { get; set; }

        [EmailAddress, MaxLength(120)]
        public string? Correo { get; set; }

        [MaxLength(20)]
        public string? Telefono { get; set; }

        [MaxLength(80)]
        public string? Puesto { get; set; }

        [Range(0, 9_999_999)]
        public decimal Salario { get; set; } = 0m;

        public DateTime? Fecha_Ingreso { get; set; }

        /// <summary>
        /// 'Activo' | 'Inactivo' | 'Baja'
        /// </summary>
        [Required, MaxLength(10)]
        public string Estatus { get; set; } = "Activo";
    }
}
