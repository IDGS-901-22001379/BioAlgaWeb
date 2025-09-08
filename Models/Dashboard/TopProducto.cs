using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioAlga.Backend.Models.Dashboard
{
    [Keyless]
    public class TopProducto
    {
        [Column("id_producto")]
        public int IdProducto { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("total_unidades")]
        public int TotalUnidades { get; set; }

        [Column("ingreso_total")]
        public decimal IngresoTotal { get; set; }
    }
}
