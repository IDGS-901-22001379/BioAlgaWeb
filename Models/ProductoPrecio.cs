using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioAlga.Backend.Models
{
    [Table("producto_precios")]
    public class ProductoPrecio
    {
        [Key] public int IdPrecio { get; set; }

        [Required] public int IdProducto { get; set; }

        [Required, MaxLength(15)]
        public string TipoPrecio { get; set; } = "Normal"; // Normal/Mayoreo/Descuento/Especial :contentReference[oaicite:5]{index=5}

        [Required] public decimal Precio { get; set; }

        public DateTime VigenteDesde { get; set; } = DateTime.UtcNow;
        public DateTime? VigenteHasta { get; set; }

        public bool Activo { get; set; } = true;  // 0..1 vigente por tipo/prod :contentReference[oaicite:6]{index=6}

        public Producto? Producto { get; set; }
    }
}
