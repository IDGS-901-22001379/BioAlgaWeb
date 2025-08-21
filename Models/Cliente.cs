using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioAlga.Backend.Models
{
    [Table("clientes")]
    public class Cliente
    {
        [Key]
        [Column("id_cliente")]
        public int IdCliente { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = null!;

        [Column("apellido_paterno")]
        public string? ApellidoPaterno { get; set; }

        [Column("apellido_materno")]
        public string? ApellidoMaterno { get; set; }

        [Column("correo")]
        public string? Correo { get; set; }

        [Column("telefono")]
        public string? Telefono { get; set; }

        [Column("direccion")]
        public string? Direccion { get; set; }

        [Column("tipo_cliente")]
        public string TipoCliente { get; set; } = "Normal";

        [Column("estado")]
        public string Estado { get; set; } = "Activo";

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}
