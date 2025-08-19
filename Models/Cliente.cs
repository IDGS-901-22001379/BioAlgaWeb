// Models/Cliente.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioAlga.Backend.Models
{
    [Table("clientes")]
    public class Cliente
    {
        [Key]
        [Column("id_cliente")]
        public int Id_Cliente { get; set; }

        [Required, MaxLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        // Un solo apellido (coincide con tu DTO y con la base actual)
        [MaxLength(100)]
        [Column("apellido")]
        public string? Apellido { get; set; }

        [MaxLength(100)]
        [EmailAddress]
        [Column("correo")]
        public string? Correo { get; set; }

        [MaxLength(20)]
        [Column("telefono")]
        public string? Telefono { get; set; }

        [Column("direccion", TypeName = "TEXT")]
        public string? Direccion { get; set; }

        // Mantén estos como string (ENUM en MySQL si lo deseas)
        [Required]
        [MaxLength(20)]
        [Column("tipo_cliente")]
        public string Tipo_Cliente { get; set; } = "Normal"; // Normal | Mayorista | Premium (ej.)

        [Required]
        [MaxLength(10)]
        [Column("estado")]
        public string Estado { get; set; } = "Activo"; // Activo | Inactivo

        [Column("fecha_registro")]
        public DateTime Fecha_Registro { get; set; } = DateTime.UtcNow;
    }
}
