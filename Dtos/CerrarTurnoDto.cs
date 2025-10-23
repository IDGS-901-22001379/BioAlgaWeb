using System.ComponentModel.DataAnnotations;

namespace BioAlga.Backend.Dtos
{
    public class CerrarTurnoDto
    {
        [Required, Range(0, 99999999)]
        public decimal SaldoCierre { get; set; }

        [StringLength(4000)]
        public string? Observaciones { get; set; }
    }
}
