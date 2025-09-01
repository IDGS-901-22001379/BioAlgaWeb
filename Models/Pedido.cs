using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using BioAlga.Backend.Models.Enums;

namespace BioAlga.Backend.Models
{
    [Table("pedidos")]
    [Index(nameof(Estatus))]
    public class Pedido
    {
        [Key]
        [Column("id_pedido")]
        public int IdPedido { get; set; }

        // Cliente obligatorio para pedidos (según tu esquema)
        [Column("cliente_id")]
        public int IdCliente { get; set; }

        // Usuario que creó/gestiona el pedido
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("fecha_pedido")]
        public DateTime FechaPedido { get; set; } = DateTime.Now;

        [Column("fecha_requerida")]
        public DateTime? FechaRequerida { get; set; }

        [Precision(12, 2)]
        [Column("anticipo", TypeName = "decimal(12,2)")]
        public decimal Anticipo { get; set; } = 0m;

        [Precision(12, 2)]
        [Column("subtotal", TypeName = "decimal(12,2)")]
        public decimal Subtotal { get; set; } = 0m;

        [Precision(12, 2)]
        [Column("impuestos", TypeName = "decimal(12,2)")]
        public decimal Impuestos { get; set; } = 0m;

        [Precision(12, 2)]
        [Column("total", TypeName = "decimal(12,2)")]
        public decimal Total { get; set; } = 0m;

        [Column("estatus")]
        public EstatusPedido Estatus { get; set; } = EstatusPedido.Borrador;

        [Column("notas")]
        public string? Notas { get; set; }

        // ===== Navegaciones =====
        public Cliente? Cliente { get; set; }
        public Usuario? Usuario { get; set; }

        public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
    }
}
