using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioAlga.Backend.Models
{
    [Table("devoluciones")]
    public class Devolucion
    {
        [Key]
        [Column("id_devolucion")]
        public int IdDevolucion { get; set; }

        [Column("fecha_devolucion")]
        public DateTime FechaDevolucion { get; set; } = DateTime.UtcNow;

        // FK al usuario que registró la devolución (integridad)
        [Required]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        // Snapshot del nombre del usuario en el momento de la devolución
        [Required]
        [MaxLength(120)]
        [Column("usuario_nombre")]
        public string UsuarioNombre { get; set; } = string.Empty;

        // Motivo requerido
        [Required]
        [MaxLength(300)]
        [Column("motivo")]
        public string Motivo { get; set; } = string.Empty;

        // 1 = regresa a inventario, 0 = no (dañado, etc.)
        [Required]
        [Column("regresa_inventario")]
        public bool RegresaInventario { get; set; } = true;

        // Total que se descuenta de "ventas del día"
        [Required]
        [Column("total_devuelto", TypeName = "decimal(12,2)")]
        public decimal TotalDevuelto { get; set; }

        // Opcional: si se conoce el número/ticket de la venta
        [Column("referencia_venta")]
        [MaxLength(50)]
        public string? ReferenciaVenta { get; set; }

        [Column("notas")]
        public string? Notas { get; set; }

        // ===== Navegación =====
        [ForeignKey(nameof(IdUsuario))]
        public Usuario? Usuario { get; set; }

        public ICollection<DetalleDevolucion> Detalles { get; set; } = new List<DetalleDevolucion>();
    }
}
