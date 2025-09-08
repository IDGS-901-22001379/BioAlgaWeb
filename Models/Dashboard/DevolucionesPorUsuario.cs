using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioAlga.Backend.Models.Dashboard
{
    [Keyless]
    public class DevolucionesPorUsuario
    {
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("nombre_usuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Column("num_devoluciones")]
        public int NumDevoluciones { get; set; }

        [Column("total_devuelto")]
        public decimal TotalDevuelto { get; set; }
    }
}
