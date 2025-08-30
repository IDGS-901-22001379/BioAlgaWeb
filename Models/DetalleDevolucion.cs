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

        [Required]
        [Column("id_devolucion")]
        public int IdDevolucion { get; set; }

        [Required]
        [Column("id_producto")]
        public int IdProducto { get; set; }

        // Snapshot del nombre del producto para reporting
        [Required]
        [MaxLength(150)]
        [Column("producto_nombre")]
        public string ProductoNombre { get; set; } = string.Empty;

        [Required]
        [Column("cantidad")]
        public int Cantidad { get; set; }

        // Total por la línea (importe capturado a mano, ya con lo que tú decidas)
        [Required]
        [Column("importe_linea_total", TypeName = "decimal(12,2)")]
        public decimal ImporteLineaTotal { get; set; }

        // ===== Navegación =====
        [ForeignKey(nameof(IdDevolucion))]
        public Devolucion? Devolucion { get; set; }

        [ForeignKey(nameof(IdProducto))]
        public Producto? Producto { get; set; }
    }
}
