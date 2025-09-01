using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BioAlga.Backend.Models
{
    [Table("detalle_pedido")]
    public class DetallePedido
    {
        [Key]
        [Column("id_detalle")]
        public int IdDetalle { get; set; }

        [Column("id_pedido")]
        public int IdPedido { get; set; }

        [Column("id_producto")]
        public int IdProducto { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }

        // Precio unitario CONGELADO al confirmar el pedido
        [Precision(10, 2)]
        [Column("precio_unitario", TypeName = "decimal(10,2)")]
        public decimal PrecioUnitario { get; set; }

        // ===== Navegaciones =====
        public Pedido? Pedido { get; set; }
        public Producto? Producto { get; set; }
    }
}
