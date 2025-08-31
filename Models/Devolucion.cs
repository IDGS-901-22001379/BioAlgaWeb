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
        public DateTime FechaDevolucion { get; set; } = DateTime.Now;

        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("usuario_nombre")]
        [MaxLength(120)]
        public string UsuarioNombre { get; set; } = string.Empty;

        [Column("motivo")]
        [MaxLength(300)]
        public string Motivo { get; set; } = string.Empty;

        [Column("regresa_inventario")]
        public bool RegresaInventario { get; set; } = true;

        [Column("total_devuelto")]
        public decimal TotalDevuelto { get; set; } = 0;

        [Column("referencia_venta")]
        [MaxLength(50)]
        public string? ReferenciaVenta { get; set; }

        [Column("notas")]
        public string? Notas { get; set; }

        // 🔗 Relación opcional con venta
        [Column("venta_id")]
        public int? VentaId { get; set; }
        public Venta? Venta { get; set; }

        // 🔗 Relación con Usuario
        public Usuario? Usuario { get; set; }

        // 🔗 Relación detalle
        public ICollection<DetalleDevolucion> Detalles { get; set; } = new List<DetalleDevolucion>();
    }
}
