// Models/DetalleVenta.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioAlga.Backend.Models
{
    [Table("detalle_venta")]
    public class DetalleVenta
    {
        [Key]
        [Column("id_detalle")]
        public int IdDetalle { get; set; }

        [Column("id_venta")]
        public int IdVenta { get; set; }

        [Column("id_producto")]
        public int IdProducto { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Column("precio_unitario", TypeName = "decimal(10,2)")]
        public decimal PrecioUnitario { get; set; }

        [Column("descuento_unitario", TypeName = "decimal(10,2)")]
        public decimal DescuentoUnitario { get; set; }

        [Column("iva_unitario", TypeName = "decimal(10,2)")]
        public decimal IvaUnitario { get; set; }

        // ------------ Navegación ------------
        public Venta? Venta { get; set; }
        public Producto? Producto { get; set; }

        // ------------ Calculados (no mapeados) ------------
        [NotMapped]
        public decimal ImporteBruto => PrecioUnitario * Cantidad;

        [NotMapped]
        public decimal ImporteDescuento => DescuentoUnitario * Cantidad;

        [NotMapped]
        public decimal ImporteIva => IvaUnitario * Cantidad;

        [NotMapped]
        public decimal ImporteNeto => ImporteBruto - ImporteDescuento + ImporteIva;
    }
}
