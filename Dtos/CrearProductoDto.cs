using System.ComponentModel.DataAnnotations;
using BioAlga.Backend.Models.Enums;

namespace BioAlga.Backend.Dtos
{
    public class CrearProductoDto
    {
        [Required, MinLength(3), MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        [Required]
        public TipoProducto Tipo { get; set; }

        public int? IdCategoria { get; set; }
        public int? IdMarca { get; set; }
        public int? IdUnidad { get; set; }
        public int? ProveedorPreferenteId { get; set; }

        [Required, MinLength(1), MaxLength(40)]
        public string CodigoSku { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? CodigoBarras { get; set; }

        public string Estatus { get; set; } = "Activo";
    }
}
