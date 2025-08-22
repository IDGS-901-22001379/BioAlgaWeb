// Models/Empleado.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BioAlga.Backend.Models
{
    [Table("empleados")]
    [Index(nameof(Correo), IsUnique = false)]
    [Index(nameof(Nombre), nameof(Apellido_Paterno), nameof(Apellido_Materno))]
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

        [MaxLength(18)]
        [Column("curp")]
        public string? Curp { get; set; }

        [MaxLength(13)]
        [Column("rfc")]
        public string? Rfc { get; set; }

        [MaxLength(120)]
        [Column("correo")]
        public string? Correo { get; set; }

        [MaxLength(20)]
        [Column("telefono")]
        public string? Telefono { get; set; }

        [MaxLength(80)]
        [Column("puesto")]
        public string? Puesto { get; set; }

        [Column("salario", TypeName = "decimal(10,2)")]
        public decimal Salario { get; set; } = 0m;

        [Column("fecha_ingreso", TypeName = "date")]
        public DateTime? Fecha_Ingreso { get; set; }

        [Column("fecha_baja", TypeName = "date")]
        public DateTime? Fecha_Baja { get; set; }

        [Required, MaxLength(10)]
        [Column("estatus")]
        public string Estatus { get; set; } = "Activo";

        [Column("created_at")]
        public DateTime Created_At { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime Updated_At { get; set; } = DateTime.UtcNow;

        // Relación 1–1 con Usuario
        public virtual Usuario? Usuario { get; set; }
    }
}
