// Models/Usuario.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BioAlga.Backend.Models
{
    [Table("usuarios")]
    [Index(nameof(Nombre_Usuario), IsUnique = true)]
    public class Usuario
    {
        [Key]
        [Column("id_usuario")]
        public int Id_Usuario { get; set; }

        [Required, MaxLength(50)]
        [Column("nombre_usuario")]
        public string Nombre_Usuario { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        [Column("contrasena")]
        public string Contrasena { get; set; } = string.Empty;

        [Required]
        [Column("id_rol")]
        public int Id_Rol { get; set; }

        [Column("id_empleado")]
        public int? Id_Empleado { get; set; }

        [Column("ultimo_login")]
        public DateTime? Ultimo_Login { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Column("fecha_registro")]
        public DateTime Fecha_Registro { get; set; } = DateTime.UtcNow;

        // Navegaciones
        public virtual Rol? Rol { get; set; }
        public virtual Empleado? Empleado { get; set; }
    }

    [Table("roles")]
    public class Rol
    {
        [Key]
        [Column("id_rol")]
        public int Id_Rol { get; set; }

        [Required, MaxLength(40)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(200)]
        [Column("descripcion")]
        public string? Descripcion { get; set; }
    }

    [Table("empleados")]
    public class Empleado
    {
        [Key]
        [Column("id_empleado")]
        public int Id_Empleado { get; set; }

        [Required, MaxLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column("apellido_paterno")]
        public string? Apellido_Paterno { get; set; }

        [MaxLength(100)]
        [Column("apellido_materno")]
        public string? Apellido_Materno { get; set; }

        [MaxLength(120)]
        [Column("correo")]
        public string? Correo { get; set; }
    }
}
