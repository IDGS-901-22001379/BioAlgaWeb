using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioAlga.Backend.Models
{
    [Table("detalle_devolucion")]
    public class DetalleDevolucion
    {
        [Key]
        [Column("id_detalle")]
        public int IdDetalle { get; set; }

        [Column("id_devolucion")]
        public int IdDevolucion { get; set; }

        [Column("id_producto")]
        public int IdProducto { get; set; }

        [Column("producto_nombre")]
        [MaxLength(150)]
        public string ProductoNombre { get; set; } = string.Empty;

        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Column("importe_linea_total")]
        public decimal ImporteLineaTotal { get; set; }

        // 🔗 Relación con devolución
        public Devolucion? Devolucion { get; set; }

        // 🔗 Relación con producto
        public Producto? Producto { get; set; }

        // 🔗 Relación opcional con detalle de venta
        [Column("id_detalle_venta")]
        public int? IdDetalleVenta { get; set; }
        public DetalleVenta? DetalleVenta { get; set; }
    }
}
