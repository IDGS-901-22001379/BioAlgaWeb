using System.ComponentModel.DataAnnotations;

namespace BioAlga.Backend.Dtos
{
    public class CrearPrecioDto
    {
        [Required] public string TipoPrecio { get; set; } = "Normal";
        [Required] public decimal Precio { get; set; }
        public DateTime? VigenteDesde { get; set; }   // si null → Now
        public DateTime? VigenteHasta { get; set; }   // opcional
        public bool Activo { get; set; } = true;
    }
}
