using System;
using System.Collections.Generic;
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

        // FK opcional hacia clientes
        [Column("cliente_id")]
        public int? ClienteId { get; set; }

        [Required]
        [Column("fecha_venta")]
        public DateTime FechaVenta { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("subtotal", TypeName = "decimal(10,2)")]
        public decimal Subtotal { get; set; }

        [Required]
        [Column("impuestos", TypeName = "decimal(10,2)")]
        public decimal Impuestos { get; set; }

        [Required]
        [Column("total", TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        [Column("efectivo_recibido", TypeName = "decimal(10,2)")]
        public decimal? EfectivoRecibido { get; set; }

        [Column("cambio", TypeName = "decimal(10,2)")]
        public decimal? Cambio { get; set; }

        [Required]
        [Column("metodo_pago")]
        public MetodoPago MetodoPago { get; set; } = MetodoPago.Efectivo;

        // FK hacia usuarios
        [Required]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Required]
        [Column("estatus")]
        public EstatusVenta Estatus { get; set; } = EstatusVenta.Pagada;

        // =======================
        // Navegaciones
        // =======================
        [ForeignKey(nameof(ClienteId))]
        public virtual Cliente? Cliente { get; set; }

        [ForeignKey(nameof(IdUsuario))]
        public virtual Usuario? Usuario { get; set; }

        public virtual ICollection<DetalleVenta> Detalle { get; set; } = new List<DetalleVenta>();
    }
}
