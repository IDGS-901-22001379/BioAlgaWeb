using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioAlga.Backend.Models
{
    [Table("detalle_ventas")]
    public class DetalleVenta
    {
        [Key]
        [Column("id_detalle")]
        public int IdDetalle { get; set; }

        [Required]
        [Column("id_venta")]
        public int IdVenta { get; set; }

        [Required]
        [Column("id_producto")]
        public int IdProducto { get; set; }

        [Required]
        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Required]
        [Column("precio_unitario", TypeName = "decimal(10,2)")]
        public decimal PrecioUnitario { get; set; }

        [Column("descuento_unitario", TypeName = "decimal(10,2)")]
        public decimal DescuentoUnitario { get; set; } = 0m;

        [Column("iva_unitario", TypeName = "decimal(10,2)")]
        public decimal IvaUnitario { get; set; } = 0m;

        // =======================
        // Navegaciones
        // =======================
        [ForeignKey(nameof(IdVenta))]
        public virtual Venta Venta { get; set; } = null!;

        [ForeignKey(nameof(IdProducto))]
        public virtual Producto Producto { get; set; } = null!;
    }
}
