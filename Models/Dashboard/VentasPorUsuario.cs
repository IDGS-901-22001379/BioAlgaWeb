using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioAlga.Backend.Models.Dashboard
{
    [Keyless]
    public class VentasPorUsuario
    {
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("apellido_paterno")]
        public string ApellidoPaterno { get; set; } = string.Empty;

        [Column("total_vendido")]
        public decimal TotalVendido { get; set; }

        [Column("num_ventas")]
        public int NumVentas { get; set; }
    }
}
