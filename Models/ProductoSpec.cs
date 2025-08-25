using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioAlga.Backend.Models
{
    [Table("producto_specs")]
    public class ProductoSpec
    {
        [Key] public int IdSpec { get; set; }

        [Required]
        public int IdProducto { get; set; }

        [Required, MaxLength(80)]
        public string Clave { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string Valor { get; set; } = string.Empty; // clave-valor (Voltaje:5V, Chip:ESP32…) :contentReference[oaicite:4]{index=4}

        public Producto? Producto { get; set; }
    }
}
