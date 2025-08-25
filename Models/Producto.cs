using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BioAlga.Backend.Models.Enums;

namespace BioAlga.Backend.Models
{
    [Table("productos")]
    public class Producto
    {
        [Key] public int IdProducto { get; set; }

        [Required, MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        [Required]
        public TipoProducto Tipo { get; set; } = TipoProducto.Componente; // :contentReference[oaicite:2]{index=2}

        public int? IdCategoria { get; set; }
        public int? IdMarca { get; set; }
        public int? IdUnidad { get; set; }
        public int? ProveedorPreferenteId { get; set; }

        [Required, MaxLength(40)]
        public string CodigoSku { get; set; } = string.Empty; // único

        [MaxLength(50)]
        public string? CodigoBarras { get; set; }

        [Required, MaxLength(10)]
        public string Estatus { get; set; } = "Activo";       // no vender Inactivo :contentReference[oaicite:3]{index=3}

        public DateTime Created_At { get; set; } = DateTime.UtcNow;
        public DateTime Updated_At { get; set; } = DateTime.UtcNow;

        // Navs (opcionales si tienes modelos Cat/Marca/Unidad/Proveedor)
        public ICollection<ProductoSpec> Especificaciones { get; set; } = new List<ProductoSpec>();
        public ICollection<ProductoPrecio> Precios { get; set; } = new List<ProductoPrecio>();
    }
}
