using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioAlga.Backend.Models.Dashboard
{
    [Keyless]
    public class TopCliente
    {
        [Column("id_cliente")]
        public int IdCliente { get; set; }

        [Column("nombre_completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Column("total_gastado")]
        public decimal TotalGastado { get; set; }
    }
}
