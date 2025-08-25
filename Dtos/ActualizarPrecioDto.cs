using System.ComponentModel.DataAnnotations;

namespace BioAlga.Backend.Dtos
{
    public class ActualizarPrecioDto
    {
        [Required] public decimal Precio { get; set; }
        public DateTime? VigenteHasta { get; set; }
        public bool? Activo { get; set; }
    }
}
