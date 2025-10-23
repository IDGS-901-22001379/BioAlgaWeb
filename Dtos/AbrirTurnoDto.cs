using System.ComponentModel.DataAnnotations;

namespace BioAlga.Backend.Dtos
{
    public class AbrirTurnoDto
    {
        [Required, StringLength(50)]
        public string NombreUsuario { get; set; } = default!;

        [Required, StringLength(50)]
        public string NombreCaja { get; set; } = default!;

        [Range(0, 99999999)]
        public decimal SaldoInicial { get; set; } = 0m;

        [StringLength(4000)]
        public string? Observaciones { get; set; }

        // Opcional: si la caja no existe y quieres crearla con descripción
        [StringLength(150)]
        public string? DescripcionCaja { get; set; }
    }
}
