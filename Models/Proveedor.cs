using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioAlga.Backend.Models
{
    [Table("proveedores")]
    public class Proveedor
    {
        [Key]
        [Column("id_proveedor")]
        public int IdProveedor { get; set; }

        [Required, MaxLength(120)]
        [Column("nombre_empresa")]
        public string NombreEmpresa { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column("contacto")]
        public string? Contacto { get; set; }

        [MaxLength(120)]
        [Column("correo")]
        public string? Correo { get; set; }

        [MaxLength(20)]
        [Column("telefono")]
        public string? Telefono { get; set; }

        [Column("direccion")]
        public string? Direccion { get; set; }

        [MaxLength(13)]
        [Column("rfc")]
        public string? Rfc { get; set; }

        [MaxLength(50)]
        [Column("pais")]
        public string? Pais { get; set; }

        [MaxLength(50)]
        [Column("ciudad")]
        public string? Ciudad { get; set; }

        [MaxLength(10)]
        [Column("codigo_postal")]
        public string? CodigoPostal { get; set; }

        [Required]
        [Column("estatus")]
        public string Estatus { get; set; } = "Activo"; // Activo/Inactivo

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

