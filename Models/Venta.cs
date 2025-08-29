// Models/Venta.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BioAlga.Backend.Models.Enums;

namespace BioAlga.Backend.Models
{
    [Table("ventas")]
    public class Venta
    {
        [Key]
        [Column("id_venta")]
        public int IdVenta { get; set; }

        [Column("cliente_id")]
        public int? ClienteId { get; set; }

        [Column("fecha_venta")]
        public DateTime FechaVenta { get; set; } = DateTime.UtcNow;

        [Column("subtotal", TypeName = "decimal(12,2)")]
        public decimal Subtotal { get; set; }

        [Column("impuestos", TypeName = "decimal(12,2)")]
        public decimal Impuestos { get; set; }

        [Column("total", TypeName = "decimal(12,2)")]
        public decimal Total { get; set; }

        [Column("efectivo_recibido", TypeName = "decimal(12,2)")]
        public decimal? EfectivoRecibido { get; set; }

        [Column("cambio", TypeName = "decimal(12,2)")]
        public decimal? Cambio { get; set; }

        [Column("metodo_pago")]
        public MetodoPago MetodoPago { get; set; } = MetodoPago.Efectivo;

        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("estatus")]
        public EstatusVenta Estatus { get; set; } = EstatusVenta.Pagada;

        // ------------ Navegación ------------
        public Cliente? Cliente { get; set; }
        public Usuario? Usuario { get; set; }

        public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();

        // ------------ Calculados útiles (no mapeados) ------------
        [NotMapped]
        public int TotalPartidas => Detalles?.Count ?? 0;

        [NotMapped]
        public int TotalUnidades => Detalles?.Sum(d => d.Cantidad) ?? 0;
    }
}
